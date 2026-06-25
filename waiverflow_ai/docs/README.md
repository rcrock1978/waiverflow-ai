# WaiverFlow AI

Autonomous lien-waiver and compliance-document tracker for GCs and subs.

## Architecture

Multi-service .NET 10 backend with a Python AI microservice for OCR document
validation. See `specs/001-core-foundation/plan.md` for full architecture.

## Quick Start

```bash
docker compose up -d postgres redis
dotnet run --project src/WaiverFlow.Api
```

## Projects

| Project | Responsibility |
|---------|---------------|
| WaiverFlow.Api | API Gateway, auth, middleware |
| WaiverFlow.DocumentRequests | Waiver generation, project/sub management |
| WaiverFlow.Validation | Document upload, OCR orchestration |
| WaiverFlow.Compliance | COI tracking, state waiver rules |
| WaiverFlow.Collaboration | Notifications, escalation, reminders |
| WaiverFlow.Reporting | Pay-app readiness, audit exports |
| WaiverFlow.Shared | DDD primitives, tenant context, audit, middleware |
| WaiverFlow.AI (Python) | OCR document extraction and validation |

## Logging

All services use structured JSON logging via Serilog (.NET) and structlog (Python).
CRUD create payloads are logged to `logs/payloads/{ServiceName}/{EntityId}-{yyyy-MM-dd}.json`.
