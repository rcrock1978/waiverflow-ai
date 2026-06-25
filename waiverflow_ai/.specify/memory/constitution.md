<!--
  Sync Impact Report

  Version change: — → 1.0.0 (initial constitution)
  Modified principles: (new) all 5 principles created
  Added sections:
    - Core Principles (I through V)
    - Technology & Architecture Constraints
    - Development Workflow
    - Governance
  Removed sections: none
  Templates requiring updates:
    - .specify/templates/plan-template.md ✅ already has generic "Constitution Check" / "Complexity Tracking" sections — no change needed
    - .specify/templates/spec-template.md ✅ already has user stories, requirements, success criteria sections — no change needed
    - .specify/templates/tasks-template.md ✅ already has setup/foundational/user-story/polish phases — no change needed
  Follow-up TODOs: none
-->

# WaiverFlow AI Constitution

## Core Principles

### I. Spec-First Development (SDD)
The feature specification (`spec.md`) is the single source of truth for every feature.
All code, tests, contracts, and documentation MUST be generated from and validated
against the spec. No implementation work begins until the spec is reviewed and
approved. Spec changes MUST be reflected in all downstream artifacts (plan, tasks,
code, tests) before they are considered done.

### II. Clean Architecture & Domain-Driven Design
The codebase MUST follow Clean Architecture with strict dependency rules:
dependencies point inward. The Domain layer MUST have zero framework dependencies.
Each bounded context (Document Requests, Validation, Compliance Rules,
Collaboration, Reporting) MUST own its data and behavior. Anti-Corruption Layers
MUST protect domain boundaries from third-party models. Architecture boundaries
MUST be enforced by automated tooling (ArchUnitNET / solution layering tests).

### III. CQRS & Event-Driven Architecture
Commands and queries MUST be separated. Commands enforce invariants on aggregates
and emit domain events. Queries MUST read from denormalized read models and MUST
NOT mutate state. Domain events MUST be published reliably via the transactional
outbox pattern. Integration events MUST flow through a message bus with idempotent
consumers and dead-letter queues. Sagas/process managers coordinate multi-step
cross-service workflows.

### IV. AI Quality & Safety
All AI-generated output MUST be grounded in retrieved context and pass automated
evaluation thresholds (relevance, faithfulness, task success) before being surfaced
to users or acted upon. Every LLM/agent/tool call MUST be traced (tokens, cost,
latency, outcome). Prompt-injection defense, PII redaction, and output moderation
guardrails MUST be applied to every AI interaction. No AI change MAY ship without
passing CI eval gates.

### V. Multi-Tenancy & Security by Design
Every table MUST carry a tenant key. Row-level security MUST enforce tenant
isolation at the database level. AuthN MUST use OpenID Connect (OIDC/OAuth2);
AuthZ MUST use role- and attribute-based access control. All state-changing
operations MUST be logged to an append-only audit log. Secrets MUST NEVER appear
in code or images. Data MUST be encrypted in transit (TLS 1.2+) and at rest
(AES-256). Supply chain security (SCA/SAST/secret scanning, signed images, SBOM)
MUST be enforced in CI/CD.

## Technology & Architecture Constraints

### Stack (mandatory unless overridden by an approved ADR)
- .NET 10 / C# 14 for all bounded-context services
- ASP.NET Core (minimal APIs + controllers), MediatR, FluentValidation
- PostgreSQL via EF Core (primary operational store)
- pgvector for semantic search over statutes and templates
- Redis for caching (CQRS read models, session, rate-limit counters)
- Azure Service Bus or RabbitMQ via MassTransit for async messaging
- Python 3.12 for the AI inference microservice
- Semantic Kernel (.NET) + LangChain/LlamaIndex (Python) for LLM orchestration
- OpenTelemetry for all observability signals
- Docker + Kubernetes (AKS) for container orchestration
- Terraform (or Bicep) for infrastructure as code

### Architectural invariants
- The outbox pattern MUST be used for reliable event publication
- The API layer MUST use idempotency keys on all commands
- External integrations MUST be wrapped in an Anti-Corruption Layer
- All services MUST be stateless (state externalized to PostgreSQL + Redis)
- Multi-layer caching (Redis + HTTP/CDN) with cache-aside and event-driven invalidation

### Performance budgets
- API reads: p95 < 200 ms
- API writes: p95 < 400 ms
- AI first-token: < 1.5 s
- AI grounded answer: < 6 s

## Development Workflow

### SDD lifecycle (per feature)
1. `/speckit.specify` — author the spec from business intent
2. `/speckit.clarify` — resolve ambiguities in the spec
3. `/speckit.plan` — architecture decisions, data model, contracts, quickstart
4. `/speckit.tasks` — decompose into dependency-ordered tasks
5. `/speckit.implement` — execute tasks per the plan
6. `/speckit.converge` — close gaps between spec and implemented code

### CI/CD gates
Every PR MUST pass:
- Build + unit/integration tests
- AI eval suite (relevance, faithfulness, task success)
- SAST/SCA/secret scanning
- Container image build + sign (cosign)
- Tenant-isolation tests

Database migrations MUST be backward-compatible (expand-contract pattern).
Rollbacks MUST be automated on failed health or eval gates.

### Review requirements
- Constitution compliance MUST be verified in every code review
- Architecture boundary violations (inward dependency rule breaks) are automatic
  rejection criteria
- Complexity MUST be justified in the Complexity Tracking section of the plan
  when a Constitution Check violation is identified

## Governance

- This Constitution supersedes all other development practices and guidelines
- Amendments must be documented in a GitHub issue, approved, and include a
  migration plan for affected features
- Version numbering: MAJOR for backward-incompatible governance or principle
  changes; MINOR for new principles or materially expanded guidance; PATCH for
  clarifications, wording, and non-semantic refinements
- Compliance is verified by `/speckit.analyze` before implementation and by
  code review on every PR

**Version**: 1.0.0 | **Ratified**: 2026-06-26 | **Last Amended**: 2026-06-26
