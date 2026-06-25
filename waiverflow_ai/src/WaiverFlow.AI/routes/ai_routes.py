"""AI agent, RAG, and LLM API routes."""

import structlog
from fastapi import APIRouter, HTTPException
from pydantic import BaseModel

log = structlog.get_logger()
router = APIRouter(prefix="/ai")


class CompletionRequest(BaseModel):
    prompt: str
    system: str | None = None
    task_type: str = "simple"


class RAGQueryRequest(BaseModel):
    question: str
    top_k: int = 5


class AgentRunRequest(BaseModel):
    task: str


class GuardrailCheckRequest(BaseModel):
    text: str


def _get_llm():
    from services.llm import LLMOrchestrator, OpenAIProvider
    orch = LLMOrchestrator()
    orch.register("frontier", OpenAIProvider(api_key="", model="gpt-4o"))
    orch.register("fast", OpenAIProvider(api_key="", model="gpt-4o-mini"))
    return orch


def _get_rag(llm=None):
    from services.rag import EmbeddingService, VectorStore, RAGPipeline
    embedder = EmbeddingService()
    store = VectorStore()
    store.load([
        {"source": "TX-lien-law", "content": "Texas requires a conditional lien waiver. 30-day grace period. No notarization.", "embedding": [0.0]*1536},
        {"source": "CA-lien-law", "content": "California uses unconditional waivers at each progress payment. 90-day grace period.", "embedding": [0.0]*1536},
    ])
    return RAGPipeline(embedder, store, llm)


@router.post("/complete")
async def llm_complete(req: CompletionRequest):
    llm = _get_llm()
    result = await llm.route(req.prompt, req.system, req.task_type)
    return {"content": result.content, "model": result.model, "provider": result.provider}


@router.post("/rag/query")
async def rag_query(req: RAGQueryRequest):
    llm = _get_llm()
    rag = _get_rag(llm)
    result = await rag.query(req.question, req.top_k)
    return result


@router.post("/agent/run")
async def agent_run(req: AgentRunRequest):
    from services.mcp_tools import get_all_tools
    from services.agent import Agent
    llm = _get_llm()
    tools = get_all_tools()
    agent = Agent(tools, llm)
    result = await agent.run(req.task)
    return result


@router.get("/agent/tools")
async def agent_tools():
    from services.mcp_tools import get_all_tools
    tools = get_all_tools()
    return {"tools": [t.to_mcp_schema() for t in tools]}


@router.post("/guardrails/check")
async def guardrails_check(req: GuardrailCheckRequest):
    from services.guardrails import GuardrailPipeline
    input_check = await GuardrailPipeline.check_input(req.text)
    output_check = await GuardrailPipeline.check_output(req.text)
    return {"input": input_check, "output": output_check}
