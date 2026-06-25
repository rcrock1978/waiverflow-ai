"""Provider-agnostic LLM adapter with routing, fallback, and telemetry."""

import structlog
from abc import ABC, abstractmethod
from dataclasses import dataclass
from typing import Any

log = structlog.get_logger()


@dataclass
class LLMResult:
    content: str
    model: str
    provider: str
    input_tokens: int = 0
    output_tokens: int = 0
    latency_ms: float = 0.0


class LLMProvider(ABC):
    @abstractmethod
    async def complete(self, prompt: str, system: str | None = None, **kwargs) -> LLMResult:
        ...


class OpenAIProvider(LLMProvider):
    def __init__(self, api_key: str, model: str = "gpt-4o"):
        self.api_key = api_key
        self.model = model

    async def complete(self, prompt: str, system: str | None = None, **kwargs) -> LLMResult:
        import time, openai
        start = time.monotonic()
        client = openai.AsyncOpenAI(api_key=self.api_key)
        messages = []
        if system:
            messages.append({"role": "system", "content": system})
        messages.append({"role": "user", "content": prompt})
        resp = await client.chat.completions.create(model=self.model, messages=messages, **kwargs)
        elapsed = (time.monotonic() - start) * 1000
        return LLMResult(
            content=resp.choices[0].message.content or "",
            model=self.model,
            provider="openai",
            input_tokens=resp.usage.prompt_tokens if resp.usage else 0,
            output_tokens=resp.usage.completion_tokens if resp.usage else 0,
            latency_ms=elapsed,
        )


class AzureProvider(LLMProvider):
    def __init__(self, endpoint: str, api_key: str, model: str = "gpt-4o"):
        self.endpoint = endpoint
        self.api_key = api_key
        self.model = model

    async def complete(self, prompt: str, system: str | None = None, **kwargs) -> LLMResult:
        import time, openai
        start = time.monotonic()
        client = openai.AsyncAzureOpenAI(azure_endpoint=self.endpoint, api_key=self.api_key, api_version="2024-10-01-preview")
        messages = []
        if system:
            messages.append({"role": "system", "content": system})
        messages.append({"role": "user", "content": prompt})
        resp = await client.chat.completions.create(model=self.model, messages=messages, **kwargs)
        elapsed = (time.monotonic() - start) * 1000
        return LLMResult(
            content=resp.choices[0].message.content or "",
            model=self.model,
            provider="azure",
            input_tokens=resp.usage.prompt_tokens if resp.usage else 0,
            output_tokens=resp.usage.completion_tokens if resp.usage else 0,
            latency_ms=elapsed,
        )


class LLMOrchestrator:
    """Routes requests to the appropriate provider and model based on task complexity."""

    def __init__(self):
        self._providers: dict[str, LLMProvider] = {}

    def register(self, name: str, provider: LLMProvider):
        self._providers[name] = provider

    async def route(
        self, prompt: str, system: str | None = None,
        task_type: str = "simple", **kwargs
    ) -> LLMResult:
        """Route to a small model for simple tasks, frontier model for complex."""
        provider_name = "frontier" if task_type == "complex" else "fast"
        provider = self._providers.get(provider_name)
        if not provider:
            provider = list(self._providers.values())[0]
        log.info("llm.route", provider=provider_name, task=task_type)
        return await provider.complete(prompt, system, **kwargs)
