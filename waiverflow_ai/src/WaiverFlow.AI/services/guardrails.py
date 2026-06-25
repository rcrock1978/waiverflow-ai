"""AI guardrails: prompt-injection defense, PII redaction, output moderation, eval gates."""

import structlog
import re
from typing import Any

log = structlog.get_logger()


# Sensitive patterns for PII redaction
PII_PATTERNS = {
    "email": re.compile(r"\b[\w.+-]+@[\w-]+\.[\w.-]+\b"),
    "phone": re.compile(r"\b\d{3}[-.]?\d{3}[-.]?\d{4}\b"),
    "ssn": re.compile(r"\b\d{3}-\d{2}-\d{4}\b"),
    "credit_card": re.compile(r"\b(?:\d{4}[-\s]?){3}\d{4}\b"),
}


class PromptInjectionDetector:
    """Detects common prompt injection / jailbreak attempts."""

    SENSITIVE_PATTERNS = [
        "ignore previous instructions",
        "ignore all instructions",
        "forget your instructions",
        "you are now",
        "system prompt",
        "do not follow",
        "disregard",
        "new instructions",
        "override",
        "DAN",
        "jailbreak",
        "pretend",
    ]

    @classmethod
    def is_suspicious(cls, text: str) -> tuple[bool, list[str]]:
        lower = text.lower()
        hits = [p for p in cls.SENSITIVE_PATTERNS if p in lower]
        if hits:
            log.warning("guardrail.injection_detected", patterns=hits)
        return len(hits) > 0, hits


class PIIRedactor:
    @classmethod
    def redact(cls, text: str, replace_with: str = "[REDACTED]") -> str:
        result = text
        for name, pattern in PII_PATTERNS.items():
            matches = pattern.findall(result)
            if matches:
                log.info("guardrail.pii_redacted", pattern=name, count=len(matches))
                result = pattern.sub(replace_with, result)
        return result


class OutputModerator:
    HARMFUL_CATEGORIES = [
        "violence", "self-harm", "hate speech", "harassment",
        "illegal activity", "sexual content",
    ]

    @classmethod
    def is_safe(cls, text: str) -> tuple[bool, list[str]]:
        lower = text.lower()
        hits = [c for c in cls.HARMFUL_CATEGORIES if c in lower]
        if hits:
            log.warning("guardrail.content_blocked", categories=hits)
        return len(hits) == 0, hits


class EvalGate:
    """Checks if AI output meets quality thresholds before surfacing."""

    @staticmethod
    def check_basic(output: str, min_length: int = 10) -> dict[str, Any]:
        issues = []
        if len(output.strip()) < min_length:
            issues.append("Output too short")
        if len(output) > 10000:
            issues.append("Output exceeds max length")
        passed = len(issues) == 0
        log.info("guardrail.eval_gate", passed=passed, issues=issues)
        return {"passed": passed, "issues": issues}


class GuardrailPipeline:
    """Runs all guardrails in sequence on LLM input and output."""

    @classmethod
    async def check_input(cls, text: str) -> dict[str, Any]:
        results = {}
        suspicious, patterns = PromptInjectionDetector.is_suspicious(text)
        results["injection_detected"] = suspicious
        results["injection_patterns"] = patterns

        redacted = PIIRedactor.redact(text)
        results["pii_redacted"] = redacted != text
        results["safe_text"] = redacted

        return results

    @classmethod
    async def check_output(cls, text: str) -> dict[str, Any]:
        results = {}
        safe, harmful = OutputModerator.is_safe(text)
        results["safe"] = safe
        results["harmful_categories"] = harmful

        eval_result = EvalGate.check_basic(text)
        results.update(eval_result)

        return results
