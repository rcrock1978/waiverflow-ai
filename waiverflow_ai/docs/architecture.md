# WaiverFlow AI — Architecture & Design Document

## 1. Executive Summary

**WaiverFlow AI** is a cloud-native, multi-tenant micro-SaaS platform that
automates the lien-waiver and compliance-document collection workflow for
general contractors (GCs) and subcontractors. The system replaces a manual,
spreadsheet-driven process with an intelligent agent that generates, sends,
tracks, validates, and follows up on lien waivers and certificates of insurance
(COIs) across every pay cycle.

Built with Spec-Driven Development (SDD) using GitHub Spec Kit, the project
follows Clean Architecture, Domain-Driven Design (DDD), CQRS, and Event-Driven
patterns on a .NET 10 / C# 14 backend with a Python AI microservice and an
Angular 19 SPA frontend.

---

## 2. Problem Statement

General contractors manage dozens of subcontractors per project. Every pay
cycle requires collecting signed lien waivers and valid COIs from each sub
before payment can be released. This process is:

- **Manual** — waivers are generated, sent, and tracked in spreadsheets
- **Error-prone** — wrong waiver type (conditional vs. unconditional) by state
- **Slow** — chasing overdue documents consumes hours per cycle
- **Risky** — expired COIs create liability gaps; missing waivers delay payments

---

## 3. Solution Overview

WaiverFlow AI provides an end-to-end digital workflow:

| Stage | What happens | Automation |
|-------|-------------|------------|
| 1. Onboarding | GC adds subs manually or via CSV import | Bulk import with validation |
| 2. Pay Cycle | GC starts a cycle; system generates waivers for all subs | Waiver type auto-detected by state |
| 3. Notification | Sub receives email with secure upload link | SendGrid transactional email |
| 4. Upload | Sub returns signed waiver + COI | Web portal with file upload |
| 5. Validation | OCR extracts fields; system validates signature, date, amount | Azure AI Document Intelligence |
| 6. Escalation | Overdue waivers auto-escalate at 1/3/5 days | Escalation service + reminders |
| 7. Dashboard | GC views readiness status across all subs | Real-time compliance dashboard |
| 8. Audit | GC exports ZIP package for lender/owner | Waivers + COIs + summary CSV |

---

## 4. Architecture

### 4.1 High-Level System Design

```
┌─────────────┐     ┌──────────────────────────────────────────────────────┐
│  Angular 19 │     │              API Gateway (WaiverFlow.Api)             │
│  SPA         │────>│  Auth (OIDC/DevMode) · MediatR · Swagger · Middleware│
│  :4200       │     │  Rate Limit · Idempotency · Error Handling          │
└─────────────┘     └───────┬──────────┬──────────┬──────────┬─────────────┘
                            │          │          │          │
              ┌─────────────┼──────────┼──────────┼──────────┼─────────────┐
              │             │          │          │          │             │
         ┌────▼────┐  ┌────▼────┐ ┌──▼───┐ ┌───▼───┐  ┌──▼───┐  ┌───────┐
         │Document │  │Validation│ │Compl- │ │Collab-│  │Report│  │Shared │
         │Requests │  │          │ │iance  │ │oration│  │ing   │  │Kernel │
         │ .NET    │  │ .NET     │ │ .NET  │ │ .NET  │  │ .NET │  │ .NET  │
         └────┬────┘  └────┬────┘ └──┬────┘ └──┬────┘  └──┬───┘  └───────┘
              │            │         │         │          │
              └────────────┼─────────┼─────────┼──────────┘
                           │         │         │
                    ┌──────▼─────────▼─────────▼──────┐
                    │     PostgreSQL + Redis           │
                    │     (Operational + Cache)        │
                    └──────────────────────────────────┘
                            │
                    ┌───────▼────────┐
                    │  Python AI     │  HTTP :8100
                    │  Microservice  │
                    │  OCR · LLM    │
                    │  RAG · Agent   │
                    │  Guardrails    │
                    └────────────────┘
```

### 4.2 Bounded Contexts (DDD)

| Context | Service | Responsibility |
|---------|---------|----------------|
| Document Requests | `WaiverFlow.DocumentRequests` | Projects, subs, waiver requests, pay cycles |
| Validation | `WaiverFlow.Validation` | Document upload, OCR orchestration, field validation |
| Compliance | `WaiverFlow.Compliance` | COI tracking, state waiver rules, expiry monitoring |
| Collaboration | `WaiverFlow.Collaboration` | Email notifications, escalation, reminders |
| Reporting | `WaiverFlow.Reporting` | Pay-readiness dashboard, audit export |

### 4.3 Clean Architecture Layers (per service)

```
┌──────────────────────────────────────────────┐
│                 API Layer                     │
│  Controllers · Minimal APIs · DTOs           │
├──────────────────────────────────────────────┤
│              Application Layer                │
│  MediatR Commands/Queries · Handlers          │
│  FluentValidation · Mappings                  │
├──────────────────────────────────────────────┤
│             Domain Layer                      │
│  Entities · Value Objects · Aggregates        │
│  Domain Events · Repository Interfaces        │
│  (Zero framework dependencies)                │
├──────────────────────────────────────────────┤
│           Infrastructure Layer                │
│  EF Core · PostgreSQL · Redis · SendGrid      │
│  MassTransit · Outbox · Anti-Corruption Layer │
└──────────────────────────────────────────────┘
```

### 4.4 CQRS & Event-Driven Architecture

```
Command (POST)           Query (GET)
     │                       │
     ▼                       ▼
MediatR Command         MediatR Query
     │                       │
     ▼                       ▼
Command Handler          Read Model
     │                  (Denormalized)
     ▼                       │
Aggregate Root               │
     │                       │
     ▼                       ▼
Domain Event ──► Outbox ──► Integration Event ──► Message Bus
     │                                               │
     └──────────────► Other Services consume events
```

---

## 5. Tech Stack

| Layer | Technology |
|-------|-----------|
| **Backend runtime** | .NET 10 / C# 14 |
| **Backend frameworks** | ASP.NET Core, MediatR, FluentValidation, EF Core |
| **Messaging** | MassTransit + Azure Service Bus / RabbitMQ |
| **Database** | PostgreSQL 16 + pgvector |
| **Cache** | Redis 7 |
| **AI / Python** | Python 3.12, FastAPI, Semantic Kernel, LangChain |
| **AI Providers** | Azure OpenAI / OpenAI (adapter pattern) |
| **OCR** | Azure AI Document Intelligence |
| **Frontend** | Angular 19, standalone components |
| **Email** | SendGrid (Twilio) |
| **Auth** | OpenID Connect (Auth0 / Entra ID) |
| **Observability** | OpenTelemetry, Serilog, structlog |
| **Containers** | Docker, Kubernetes (AKS) |
| **IaC** | Terraform / Bicep |

---

## 6. Data Model (Core Entities)

```
┌──────────┐     ┌──────────────┐     ┌────────────────┐
│  Tenant  │────>│   Project    │────>│ Subcontractor  │
└──────────┘     └──────────────┘     │ - CompanyName  │
       │               │              │ - ContactEmail │
       │               │              │ - WorkState    │
       │               │              │ - COIExpiryDate│
       │               │              └────────────────┘
       │               │                      │
       │               │                      ▼
       │               │              ┌────────────────┐
       │               │              │ WaiverRequest  │
       │               │              │ - Status (FSM) │
       │               │              │ - WaiverType   │
       │               │              │ - DueDate      │
       │               │              │ - EscalationLvl│
       │               │              └────────────────┘
       │               │                      │
       │               │                      ▼
       │               │              ┌────────────────┐
       │               │              │ ComplianceDoc  │
       │               │              │ - OCRStatus    │
       │               │              │ - ExtractedFlds│
       │               │              └────────────────┘
       │               │
       │               ▼
       │      ┌────────────────┐
       │      │ PayApplication │
       │      │ - Readiness    │
       │      └────────────────┘
       │
       ▼
┌──────────┐
│ AuditLog │ (Append-only)
└──────────┘

Every entity carries a TenantId for row-level security isolation.
```

**WaiverRequest State Machine:**

```
pending ──► sent ──► returned ──► validated ──► closed
  │          │          │              │
  │          │          ├──► rejected ──┤
  │          │          │              │
  │          │          └──► disputed ──┘
  │          │
  │          └──► overdue ──► escalated (1→2→3)
  │
  └──► cancelled
```

---

## 7. AI Architecture

### 7.1 Services

| AI Component | Technology | Purpose |
|-------------|-----------|---------|
| **OCR Extraction** | Azure AI Document Intelligence | Extract fields from signed waiver PDFs |
| **Document Validation** | Custom Python logic | Validate signature, date, amount, project name |
| **LLM Orchestrator** | OpenAI / Azure OpenAI | Provider-agnostic adapter with routing (small model for simple tasks, frontier for complex) |
| **RAG Pipeline** | Embeddings + Vector Search + LLM | Retrieve state lien statutes, generate grounded answers |
| **Autonomous Agent** | MCP Tool Calling + Planning | Execute collection workflows (generate waiver, validate, remind, check rules) |
| **Guardrails** | Custom detection + moderation | Prompt-injection defense, PII redaction, output safety, eval gates |

### 7.2 Agent MCP Tools

| Tool | Description |
|------|-------------|
| `generate_waiver` | Generate and send lien waiver request |
| `validate_document` | OCR-validate a returned waiver |
| `send_reminder` | Send follow-up (gentle/firm/escalation) |
| `check_state_rules` | Look up state-specific lien law |

### 7.3 Guardrail Pipeline

```
Input ──► Prompt Injection Detection ──► PII Redaction ──► LLM
Output ──► Content Moderation ──► Eval Gate (length/quality) ──► User
```

---

## 8. Logging & Observability

Every function in every file logs its entry, key decisions, and exit in
structured JSON format (Serilog in .NET, structlog in Python). CRUD create
payloads are written to `logs/payloads/{ServiceName}/{EntityId}-{yyyy-MM-dd}.json`.

```json
{
  "timestamp": "2026-06-26T10:30:00.123Z",
  "level": "info",
  "service": "WaiverFlow.DocumentRequests",
  "file": "Services/ProjectService.cs",
  "function": "CreateProject",
  "tenantId": "guid",
  "userId": "guid",
  "correlationId": "guid",
  "message": "Project created",
  "elapsedMs": 45
}
```

---

## 9. Security

- **Auth**: OpenID Connect (Auth0 / Entra ID) with bearer JWT tokens
- **Roles**: `gc_accountant`, `gc_admin`, `sub_admin`, `controller`
- **Tenant isolation**: TenantId on every entity; Row-Level Security (RLS) SQL policies
- **Audit**: Append-only audit log for all state-changing actions
- **API**: Idempotency keys on commands, rate limiting, RFC 7807 error responses
- **Data**: Encryption in transit (TLS 1.2+) and at rest (AES-256)

---

## 10. UI Architecture (Angular 19)

| Route | Page | Features |
|-------|------|----------|
| `/login` | Login | Role selection (dev mode) |
| `/projects` | Project List | CRUD cards, pay cycle modal, navigation |
| `/projects/:id/subs` | Subcontractors | Table, add form, CSV import, pay cycle |
| `/projects/:id/compliance` | COI Dashboard | Color-coded status badges |
| `/projects/:id/pay-readiness` | Pay Readiness | Stats, progress bar, export, override |
| `/submit/:waiverId` | Waiver Upload | File upload for subs |
| `/coi` | COI Upload | File upload for subs |

All API calls go through an HTTP interceptor that handles errors globally,
shows loading spinners, and displays toast notifications. Protected routes use
an Angular `AuthGuard`. Lists are paginated. Forms show validation messages.

---

## 11. Project Structure

```
waiverflow_ai/
├── specs/001-core-foundation/     # SDD artifacts (spec, plan, tasks, data-model, contracts)
│   ├── spec.md                    # Feature specification
│   ├── plan.md                    # Implementation plan
│   ├── tasks.md                   # Task breakdown
│   ├── data-model.md              # Entity definitions
│   ├── quickstart.md              # Validation guide
│   ├── contracts/                 # API endpoint contracts
│   └── checklists/                # Requirements quality checklists
├── src/
│   ├── WaiverFlow.Api/            # API Gateway (Program.cs, auth, DI)
│   ├── WaiverFlow.Shared/         # DDD primitives, tenant context, audit, middleware
│   ├── WaiverFlow.DocumentRequests/  # Projects, subs, waivers
│   ├── WaiverFlow.Validation/     # Document upload, OCR orchestration
│   ├── WaiverFlow.Compliance/     # COI tracking, state rules
│   ├── WaiverFlow.Collaboration/  # Email, escalation, reminders
│   ├── WaiverFlow.Reporting/      # Dashboard, audit export
│   ├── WaiverFlow.AI/             # Python: OCR, LLM, RAG, Agent, Guardrails
│   └── web/                       # Angular 19 SPA
├── tests/
│   ├── unit/
│   └── integration/               # Tenant isolation tests
├── infra/
│   ├── docker/                    # Docker Compose (PostgreSQL, Redis)
│   └── db/migrations/             # RLS SQL policies
├── docs/                          # Architecture documentation
└── cypress/e2e/                   # UI end-to-end tests
```

---

## 12. Development Workflow

```bash
# Start infrastructure
docker compose up -d postgres redis

# Terminal 1: .NET API (http://localhost:5000)
dotnet run --project src/WaiverFlow.Api

# Terminal 2: Python AI (http://localhost:8100)
cd src/WaiverFlow.AI && uvicorn main:app --port 8100

# Terminal 3: Angular UI (http://localhost:4200)
cd src/web && ng serve --proxy-config proxy.conf.json
```

---

## 13. Key Design Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Multi-project solution | One project per bounded context | Independent deployability, clear ownership |
| MediatR for CQRS | Decouples controllers from handlers | Testability, pipeline behaviors (validation, logging, perf) |
| In-memory services for MVP | No DB dependency in dev | Fast iteration, replaces with EF Core post-MVP |
| Python for AI | Best ecosystem for ML/LLM | Azure AI SDK, LangChain, OpenAI client |
| Angular 19 standalone | No NgModules | Simpler, tree-shakable, aligns with Angular direction |
| Custom CSS (no framework) | Zero-dependency UI | Small bundle, full control, no churn |
| Plain HTML for initial UI | Fastest first iteration | Replaced by Angular in final version |
