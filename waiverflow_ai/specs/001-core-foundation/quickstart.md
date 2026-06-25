# Quickstart: Core Waiver Flow Foundation

End-to-end validation scenarios for the core waiver flow. Run these after
implementation to verify the feature works.

## Prerequisites

- Docker and Docker Compose installed
- .NET 10 SDK + Python 3.12 installed (for local dev without containers)
- Auth0 tenant configured (or use dev-mode with stub identity)
- PostgreSQL and Redis running (via Docker Compose)

## Setup

```bash
# Start local dependencies
docker compose up -d postgres redis

# Create .NET user-secrets for dev mode
dotnet user-secrets set "Auth:DevMode" "true" --project src/WaiverFlow.Api

# Terminal 1: Start the .NET API gateway
dotnet run --project src/WaiverFlow.Api

# Terminal 2: Install Angular deps and start UI dev server
cd src/web
npm install
ng serve --proxy-config proxy.conf.json
```

The API runs on `http://localhost:5000` and the Angular UI on `http://localhost:4200`.

## Using the UI

Open `http://localhost:4200` in your browser. You'll see the login screen —
select a role to proceed (dev mode auto-authenticates).

| Page | URL | Who | What you can do |
|------|-----|-----|-----------------|
| Login | `/login` | All | Select role (dev mode) |
| Projects | `/projects` | GC accountant | List, create, navigate to subs/compliance/readiness |
| Subcontractors | `/projects/:id/subs` | GC accountant | View, add subs; CSV import; start pay cycle |
| Compliance | `/projects/:id/compliance` | GC accountant | COI expiry dashboard with color-coded status |
| Pay Readiness | `/projects/:id/pay-readiness` | GC accountant | Go/no-go status, stats, export, override |
| Submit Waiver | `/submit/:waiverId` | Sub admin | Upload signed waiver document |
| Upload COI | `/coi` | Sub admin | Upload certificate of insurance |

**Navigation flow (GC accountant)**:
1. Open `/projects` → create a project → click **Subs** on the card
2. Add subcontractors manually or import via CSV
3. Click **Start Pay Cycle** → enter label + due date
4. Go back to projects → click **Compliance** to check COI status
5. Click **Pay Readiness** to view collection progress → **Export Audit Package**

**Navigation flow (subcontractor)**:
1. Receive email notification with waiver link
2. Open `/submit/:waiverId` to upload signed waiver
3. Open `/coi` to upload certificate of insurance

## Validation Scenarios

### Scenario 1: Onboard a Project and Subs

```bash
# Create a project (authenticated as gc_admin)
curl -X POST /api/v1/projects \
  -H "Authorization: Bearer $GC_TOKEN" \
  -d '{"name": "Riverfront Tower", "description": "Phase 1 construction"}

# Add a subcontractor manually
curl -X POST /api/v1/projects/{projectId}/subcontractors \
  -H "Authorization: Bearer $GC_TOKEN" \
  -d '{"companyName": "ABC Electric", "contactEmail": "admin@abcelectric.com",
       "workState": "TX", "contactName": "John Doe"}'
```

**Expected**: Project and sub created. Sub admin receives a welcome email with
account setup link.

### Scenario 2: Start a Pay Cycle and Send Waivers

```bash
# Start a new pay cycle (generates waiver requests for all subs)
curl -X POST /api/v1/projects/{projectId}/pay-cycles \
  -H "Authorization: Bearer $GC_TOKEN" \
  -d '{"label": "June 2026 — Cycle 1", "dueDate": "2026-07-15"}'
```

**Expected**: Waiver requests created for each sub with correct waiver type
based on work state. Each sub admin receives an email with a secure link.

### Scenario 3: Sub Returns a Signed Waiver

```bash
# Sub logs in and views active waivers
curl -X GET /api/v1/sub/waivers \
  -H "Authorization: Bearer $SUB_TOKEN"

# Sub uploads signed waiver
curl -X POST /api/v1/sub/waivers/{waiverId}/submit \
  -H "Authorization: Bearer $SUB_TOKEN" \
  -F "document=@signed-waiver.pdf"
```

**Expected**: Waiver status changes to `returned`. OCR validation runs. If
valid, status changes to `validated`. If invalid, sub receives error email.

### Scenario 4: View Pay-App Readiness

```bash
curl -X GET /api/v1/projects/{projectId}/pay-readiness \
  -H "Authorization: Bearer $GC_TOKEN"
```

**Expected**: Dashboard shows each sub's waiver status, COI compliance, and
overall go/no-go indicator.

### Scenario 5: Export Audit Package

```bash
curl -X GET /api/v1/projects/{projectId}/pay-readiness/export \
  -H "Authorization: Bearer $GC_TOKEN"
```

**Expected**: Downloadable archive containing all collected waivers, COIs,
and a compliance summary.

## Scenarios Covered

| Scenario | User Story | FRs Covered | Acceptance Criteria |
|----------|------------|-------------|-------------------|
| 1 | Implicit (onboarding) | FR-016, FR-017 | Sub created, welcome sent |
| 2 | US1 | FR-001–FR-005 | Waivers generated and sent with correct type |
| 3 | US4 | FR-006–FR-008 | Document validated or rejected with feedback |
| 4 | US5 | FR-012 | Readiness dashboard reflects collection state |
| 5 | US5 | FR-013 | Audit package generated with all documents |

**COI tracking (US2)** and **autonomous follow-up (US3)** are time-dependent
scenarios that cannot be verified in a single session. They are validated via
unit tests and scheduled job integration tests.
