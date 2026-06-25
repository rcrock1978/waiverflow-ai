"""Autonomous collections agent with tool calling, planning, and memory."""

import structlog
import json
from typing import Any, Callable, Coroutine

log = structlog.get_logger()


class MCPTool:
    """A tool that the agent can call, matching the Model Context Protocol."""

    def __init__(self, name: str, description: str, parameters: dict,
                 handler: Callable[..., Coroutine[Any, Any, str]]):
        self.name = name
        self.description = description
        self.parameters = parameters
        self.handler = handler

    def to_mcp_schema(self) -> dict:
        return {
            "name": self.name,
            "description": self.description,
            "inputSchema": {
                "type": "object",
                "properties": self.parameters,
            },
        }

    async def execute(self, **kwargs) -> str:
        log.info("agent.tool_call", tool=self.name, args=kwargs)
        try:
            return await self.handler(**kwargs)
        except Exception as e:
            log.error("agent.tool_error", tool=self.name, error=str(e))
            return f"Error executing {self.name}: {e}"


class AgentMemory:
    """Short-term working memory for the agent within a single task/run."""

    def __init__(self):
        self._history: list[dict[str, Any]] = []

    def add(self, role: str, content: str, metadata: dict | None = None):
        self._history.append({"role": role, "content": content, "metadata": metadata or {}})

    def get_context(self, last_n: int = 10) -> str:
        recent = self._history[-last_n:]
        return "\n".join(f"[{m['role']}] {m['content']}" for m in recent)


class Agent:
    """Autonomous collections agent that plans, calls tools, and follows escalation policy."""

    def __init__(self, tools: list[MCPTool], llm_orchestrator=None):
        self.tools = {t.name: t for t in tools}
        self.memory = AgentMemory()
        self.llm = llm_orchestrator

    def get_mcp_schemas(self) -> list[dict]:
        return [t.to_mcp_schema() for t in self.tools.values()]

    async def run(self, task: str) -> dict[str, Any]:
        log.info("agent.run", task=task)
        self.memory.add("user", task)

        # Step 1: Plan
        plan = await self._plan(task)
        self.memory.add("assistant", f"Plan: {plan}")

        # Step 2: Execute tool calls
        results = []
        for step in plan.split("\n"):
            step = step.strip()
            if not step:
                continue
            # Parse "USE tool_name: args" from plan
            if step.upper().startswith("USE "):
                parts = step[4:].split(":", 1)
                tool_name = parts[0].strip()
                args_str = parts[1].strip() if len(parts) > 1 else "{}"
                try:
                    args = json.loads(args_str)
                except json.JSONDecodeError:
                    args = {"input": args_str}
                tool = self.tools.get(tool_name)
                if tool:
                    result = await tool.execute(**args)
                    results.append({"tool": tool_name, "result": result})
                    self.memory.add("tool", f"{tool_name}: {result}")

        # Step 3: Summarize
        summary = f"Completed {len(results)} tool calls"
        if self.llm:
            summary_result = await self.llm.route(
                f"Summarize the result of this task:\n{task}\n\nResults:\n{json.dumps(results, indent=2)}",
                system="You are a helpful assistant that summarizes task completion.",
                task_type="simple"
            )
            summary = summary_result.content

        return {"task": task, "steps": len(results), "results": results, "summary": summary}

    async def _plan(self, task: str) -> str:
        if self.llm:
            tools_desc = "\n".join(
                f"- {t.name}: {t.description}" for t in self.tools.values()
            )
            prompt = (
                f"You are a collections agent. Given a task, create a step-by-step plan\n"
                f"using the available tools. Output one step per line starting with 'USE tool_name: args'.\n\n"
                f"Available tools:\n{tools_desc}\n\nTask: {task}\n\nPlan:"
            )
            result = await self.llm.route(prompt, task_type="complex")
            return result.content
        return f"USE lookup_document: {{}}"
