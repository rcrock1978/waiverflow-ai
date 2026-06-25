# WaiverFlow AI — LinkedIn Posts

---

## Long version

🚧 I built an AI-powered lien waiver platform — here's what I learned.

General contractors manage dozens of subcontractors per project. Every pay
cycle, they chase signed lien waivers, track COI expiry dates, and pray
nothing slips through the cracks. One missing waiver = delayed payment.
One expired COI = massive liability.

I built **WaiverFlow AI** — an autonomous platform that replaces the
spreadsheet chaos with an intelligent agent that handles the entire
workflow from end to end.

**The stack:**
• .NET 10 / C# 14 backend with Clean Architecture + DDD + CQRS
• Python AI microservice (OCR, LLM, RAG, autonomous agents)
• Angular 19 SPA with real-time dashboards
• PostgreSQL + Redis + Docker + Kubernetes-ready

**What sets it apart:**
• An autonomous collections agent with MCP tool calling — generates
  waivers, validates documents, sends reminders, checks state-specific
  lien laws
• RAG pipeline over state statutes for grounded, citation-backed answers
• Multi-provider LLM adapter (OpenAI + Azure) with smart routing:
  cheap model for simple classification, frontier model for complex
  generation
• Guardrails on every AI interaction — prompt-injection detection, PII
  redaction, content moderation, eval quality gates
• Built spec-first using GitHub Spec Kit (SDD methodology) — spec is
  the source of truth, code is generated from it

**Key numbers:**
• 7 .NET microservices + 1 Python AI service + 1 Angular SPA
• 50-state lien waiver rule engine
• 5-stage escalation with automated follow-ups
• Full audit trail with ZIP export for lenders/owners
• Row-level security across all entities for multi-tenant isolation

**Biggest lesson:** Building AI into a business process isn't about
the fanciest model — it's about reliable automation with guardrails,
observability, and domain modeling that captures real-world complexity.

The PRD was my roadmap. The constitution was my compass. The spec was
my contract. The code wrote itself.

`#dotnet` `#angular` `#ai` `#microservices` `#cleancode` `#ddd`
`#opensource` `#saas` `#constructiontech`

---

## Short version

After months of building, I'm excited to share **WaiverFlow AI** 🚀

An autonomous lien-waiver and compliance tracking platform for GCs and
subcontractors. Built with:

• .NET 10 + CQRS + Clean Architecture
• Python AI microservice (OCR, RAG, LLM, autonomous agents)
• Angular 19 SPA
• PostgreSQL + Redis + Docker

The agent handles the entire pay-cycle workflow — generates waivers,
auto-detects conditional/unconditional types by state, OCR-validates
returned documents, escalates overdue items, and provides a real-time
readiness dashboard.

Built spec-first using GitHub Spec Kit. Every line of code traces back
to a spec requirement. Every AI call has guardrails.

`#dotnet` `#angular` `#ai` `#saas` `#constructiontech` `#ddd`
