---

description: "Task list for Core Waiver Flow Foundation feature implementation"

---

# Tasks: Core Waiver Flow Foundation

**Input**: Design documents from `specs/001-core-foundation/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/ (all available)

**Tests**: Test tasks not requested — validation uses quickstart.md scenarios.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Backend services**: `src/WaiverFlow.{Service}/`
- **AI microservice**: `src/WaiverFlow.AI/`
- **Web frontend (Angular)**: `src/web/`
- **Tests**: `tests/unit/`, `tests/integration/`
- **Infrastructure**: `infra/docker/`, `infra/k8s/`, `infra/terraform/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [x] T001 Create .NET solution with all service projects (DocumentRequests, Validation, Compliance, Collaboration, Reporting, Api, Shared) in `src/`
- [x] T002 [P] Add NuGet packages to each project (MediatR, EF Core, MassTransit, FluentValidation, OpenTelemetry, Serilog, Polly)
- [x] T003 [P] Create Docker Compose with PostgreSQL and Redis in `infra/docker/docker-compose.yml`
- [x] T004 [P] Create Python AI microservice scaffold with FastAPI in `src/WaiverFlow.AI/`
- [x] T005 Create shared kernel with base types (Entity, ValueObject, AggregateRoot, DomainEvent) in `src/WaiverFlow.Shared/`
- [x] T006 [P] Configure EF Core DbContexts and initial migration setup per service project
- [x] T007 Create directory structure for web frontend in `src/web/`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story

- [x] T008 Implement tenant context provider (resolves TenantId from authenticated user) in `src/WaiverFlow.Shared/`
- [x] T009 [P] Implement OIDC authentication with JWT bearer validation in `src/WaiverFlow.Api/`
- [x] T010 [P] Implement RBAC authorization policies (gc_accountant, gc_admin, sub_admin, controller) in `src/WaiverFlow.Api/`
- [x] T011 Implement Row-Level Security (RLS) database policy per tenant in each service's EF Core configuration
- [x] T026 [US1] Create sub-facing persistent account flow (welcome email, password setup, login) in `src/WaiverFlow.Api/Auth/`
- [x] T027 [US1] Build Angular 19 SPA with standalone components: project list, sub management, compliance, pay readiness, waiver submit, COI upload pages in `src/web/`

...

- [x] T032 [P] [US2] Implement sub-facing COI upload component in `src/web/src/app/components/coi/`
- [x] T033 [US2] Add COI compliance dashboard component in `src/web/src/app/components/compliance/`

...

- [x] T043 [US4] Build sub-facing waiver submission component in `src/web/src/app/components/submit/`

...

- [x] T048 [US5] Build pay-app readiness dashboard component in `src/web/src/app/components/pay-readiness/`

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [x] T049 [P] Add OpenTelemetry instrumentation and per-function JSON structured logging (Serilog with daily rolling files; structlog in Python) across all service projects
- [x] T050 [P] Add FluentValidation validators on all command endpoints in `src/WaiverFlow.Api/`
- [x] T051 [P] Add idempotency key middleware on all POST/PUT endpoints in `src/WaiverFlow.Api/Middleware/`
- [x] T052 [P] Add rate limiting middleware in `src/WaiverFlow.Api/Middleware/`
- [x] T057 [P] Implement CRUD create payload logger: writes one JSON file per create operation as `{EntityId}-{yyyy-MM-dd}.json` under `logs/payloads/{ServiceName}/`
- [x] T053 Run quickstart.md validation scenarios end-to-end and verify all pass
- [x] T054 Add solution README and service-level documentation in `docs/`
- [x] T056 [P] Add cross-tenant isolation integration test verifying TenantA user cannot access TenantB data via API in `tests/integration/`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup — BLOCKS all user stories
- **User Stories (Phase 3–7)**: All depend on Foundational completion
  - US1 (P1) → US2 (P2) → US3/4/5 (P3) in priority order
  - US3, US4, US5 can be implemented independently once US1 is complete
- **Polish (Phase 8)**: Depends on all user stories being complete

### User Story Dependencies

- **US1 (P1)**: After Foundational — no other story dependencies
- **US2 (P2)**: After Foundational — no other story dependencies
- **US3 (P3)**: After US1 (needs sent WaiverRequests to escalate)
- **US4 (P3)**: After US1 (needs sent WaiverRequests to validate against)
- **US5 (P3)**: After US1 + US2 + US4 (needs waivers, COIs, validations for readiness calc)

### Parallel Opportunities

- All [P] tasks within a phase can run in parallel
- US3, US4, and US5 implementation can run in parallel (different services)
- Once US1 completes, US2 starts; US3/US4 can start alongside US2

---

## Parallel Example: User Story 1

```bash
# Launch all entity tasks together:
T014: "Create Project entity in src/WaiverFlow.DocumentRequests/Entities/Project.cs"
T015: "Create Subcontractor entity in src/WaiverFlow.DocumentRequests/Entities/Subcontractor.cs"
T016: "Create StateWaiverRule entity in src/WaiverFlow.Compliance/Entities/StateWaiverRule.cs"

# Launch all service tasks after entities:
T019: "Implement SubcontractorService in src/WaiverFlow.DocumentRequests/Services/SubcontractorService.cs"
T020: "Implement StateWaiverRuleService in src/WaiverFlow.Compliance/Services/StateWaiverRuleService.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (BLOCKS all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Run quickstart scenarios 1 and 2
5. Deploy/demo if ready

### Incremental Delivery

1. Setup + Foundational → Foundation ready
2. Add US1 (waiver generation) → Test → Deploy/Demo (MVP!)
3. Add US2 (COI tracking) → Test → Deploy/Demo
4. Add US4 (OCR validation) → Test → Deploy/Demo
5. Add US3 (follow-up) → Test → Deploy/Demo
6. Add US5 (dashboard) → Test → Deploy/Demo

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story is independently completable and testable
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence

---

## Phase 9: Convergence

**Purpose**: Close remaining gaps between spec requirements and implemented code

- [x] T058 Implement real audit export (ZIP with waivers, COIs, compliance summary CSV) in `src/WaiverFlow.Reporting/Services/AuditExportService.cs` per FR-013 (partial)
- [x] T059 Add CSV parser with row validation and error reporting to SubcontractorService.ImportBulkAsync in `src/WaiverFlow.DocumentRequests/Services/SubcontractorImportService.cs` per FR-017 (partial)
- [x] T060 Wire Azure AI Document Intelligence real API call into OcrOrchestrator replacing SimulateOcrAsync in `src/WaiverFlow.Validation/Services/OcrOrchestrator.cs` per FR-007 (partial)
- [x] T061 Add remaining 40 state waiver rule seed data to `src/WaiverFlow.Compliance/Services/StateWaiverRuleService.cs` per SC-002 (partial)
- [x] T062 Integrate SendGrid HTTP API in EmailNotificationService for real email delivery in `src/WaiverFlow.Collaboration/Services/EmailNotificationService.cs` per FR-004 (partial)
- [x] T063 Wire real Auth0 tenant credentials or add dev-mode identity stub to skip JWT validation in `src/WaiverFlow.Api/Program.cs` and `src/WaiverFlow.Api/Auth/DevModeAuthenticationHandler.cs` per FR-018 (partial)
- [x] T064 Add EF Core migration with RLS CREATE POLICY statements per tenant in `infra/db/migrations/RLS_TenantIsolation.sql` per FR-015 (partial)
- [x] T065 Refactor ProjectsController and WaiverOverrideController to use MediatR command/query handlers in `src/WaiverFlow.DocumentRequests/Commands/` per Constitution III (partial)

---

## Phase 10: Convergence

**Purpose**: Close remaining gaps after second converge pass

- [x] T066 Create `src/WaiverFlow.AI/routes/documents.py` with `/extract` POST endpoint that calls DocumentExtractor and DocumentValidator; wire into OcrOrchestrator per FR-007 (missing)
- [x] T067 Implement DB-backed AuditLogRepository and wire into AuditLogService in `src/WaiverFlow.Shared/Services/AuditLogService.cs` per FR-015 (partial)
- [x] T068 Wire MediatR ISender into ProjectsController (replace direct ProjectService/WaiverRequestService calls with command sends) in `src/WaiverFlow.DocumentRequests/Api/ProjectsController.cs` per Constitution III (partial)

---

## Phase 11: Convergence

**Purpose**: Close final known spec gaps

- [ ] T069 Replace hard-coded GetWaiverType with call to StateWaiverRuleService.GetByStateAsync in `src/WaiverFlow.DocumentRequests/Services/WaiverRequestService.cs` per FR-003 (partial)
- [ ] T070 Add COI validation gate in CreatePayCycleAsync: skip subs with missing/expired COI and notify accountant per Edge Case 2 in spec.md (partial)
