"""Retrieval-Augmented Generation pipeline over state lien statutes and project terms."""

import structlog
import json
from typing import Any

log = structlog.get_logger()


class EmbeddingService:
    """Generates embeddings for documents and queries."""

    def __init__(self, api_key: str = "", model: str = "text-embedding-3-small"):
        self.api_key = api_key
        self.model = model

    async def embed(self, texts: list[str]) -> list[list[float]]:
        if not self.api_key:
            log.warning("rag.embed.no_api_key", msg="Using mock embeddings")
            return [[0.0] * 1536 for _ in texts]
        import openai
        client = openai.AsyncOpenAI(api_key=self.api_key)
        resp = await client.embeddings.create(model=self.model, input=texts)
        return [e.embedding for e in resp.data]


class VectorStore:
    """In-memory vector store with cosine similarity search.
       Production: backed by pgvector."""

    def __init__(self):
        self._documents: list[dict[str, Any]] = []

    def load(self, documents: list[dict[str, Any]]):
        self._documents = documents
        log.info("rag.store.loaded", count=len(documents))

    def search(self, query_embedding: list[float], top_k: int = 5) -> list[dict[str, Any]]:
        scored = []
        for doc in self._documents:
            score = self._cosine_similarity(query_embedding, doc.get("embedding", [0.0] * 1536))
            scored.append((score, doc))
        scored.sort(key=lambda x: -x[0])
        return [s[1] for s in scored[:top_k]]

    @staticmethod
    def _cosine_similarity(a: list[float], b: list[float]) -> float:
        dot = sum(ai * bi for ai, bi in zip(a, b))
        na = sum(ai * ai for ai in a) ** 0.5
        nb = sum(bi * bi for bi in b) ** 0.5
        return dot / (na * nb) if na and nb else 0.0


class RAGPipeline:
    """Multi-stage RAG: query construction → retrieval → re-ranking → generation."""

    def __init__(self, embedder: EmbeddingService, store: VectorStore, llm_orchestrator=None):
        self.embedder = embedder
        self.store = store
        self.llm = llm_orchestrator

    async def query(self, question: str, top_k: int = 5) -> dict[str, Any]:
        log.info("rag.query", question=question)

        query_embedding = (await self.embedder.embed([question]))[0]
        results = self.store.search(query_embedding, top_k=top_k)

        context = "\n\n".join(
            f"[{r.get('source', 'unknown')}] {r.get('content', '')}" for r in results
        )

        answer = None
        if self.llm:
            prompt = (
                f"Answer the question based on the provided context.\n\n"
                f"Question: {question}\n\nContext:\n{context}\n\n"
                f"Answer concisely and cite sources."
            )
            result = await self.llm.route(prompt, task_type="simple")
            answer = result.content

        return {
            "question": question,
            "answer": answer,
            "sources": [r.get("source") for r in results],
            "context": context,
        }
