"""WaiverFlow AI Microservice — OCR, LLM, RAG, Agents, Guardrails."""

import structlog
from fastapi import FastAPI
from routes.documents import router as documents_router
from routes.ai_routes import router as ai_router

log = structlog.get_logger()

app = FastAPI(title="WaiverFlow AI", version="0.2.0")
app.include_router(documents_router, prefix="/api/v1")
app.include_router(ai_router, prefix="/api/v1")


@app.on_event("startup")
async def startup():
    log.info("ai_service_started", version="0.2.0", features="ocr,llm,rag,agent,guardrails")


@app.get("/health")
async def health():
    return {"status": "ok", "version": "0.2.0"}
