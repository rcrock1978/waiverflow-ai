# API Endpoints: Core Waiver Flow Foundation

Versioned under `/api/v1`. All endpoints require authentication (OIDC bearer token)
and enforce tenant isolation via the authenticated user's TenantId.

## Projects

| Method | Path | Description |
|--------|------|-------------|
| GET | /api/v1/projects | List projects for the current tenant (paginated) |
| POST | /api/v1/projects | Create a new project |
| GET | /api/v1/projects/{id} | Get project details |
| PUT | /api/v1/projects/{id} | Update project |
| PATCH | /api/v1/projects/{id}/status | Update project status |

## Subcontractors

| Method | Path | Description |
|--------|------|-------------|
| GET | /api/v1/projects/{projectId}/subcontractors | List subs on a project (paginated) |
| POST | /api/v1/projects/{projectId}/subcontractors | Add a single sub (manual onboarding) |
| POST | /api/v1/projects/{projectId}/subcontractors/import | Bulk import subs from CSV |
| GET | /api/v1/projects/{projectId}/subcontractors/{id} | Get sub details |
| PUT | /api/v1/projects/{projectId}/subcontractors/{id} | Update sub |
| DELETE | /api/v1/projects/{projectId}/subcontractors/{id} | Remove sub from project |

## Waiver Requests

| Method | Path | Description |
|--------|------|-------------|
| POST | /api/v1/projects/{projectId}/pay-cycles | Start a new pay cycle (creates waiver requests) |
| GET | /api/v1/projects/{projectId}/waiver-requests | List waiver requests for a project (paginated, filterable by status) |
| GET | /api/v1/waiver-requests/{id} | Get waiver request details |
| POST | /api/v1/waiver-requests/{id}/override | Accountant override (mark as received manually) |
| POST | /api/v1/waiver-requests/{id}/resend | Resend waiver request to sub |

## Document Submission (Sub-facing)

| Method | Path | Description |
|--------|------|-------------|
| GET | /api/v1/sub/waivers | List active waiver requests for the authenticated sub admin |
| POST | /api/v1/sub/waivers/{id}/submit | Upload signed waiver document |
| GET | /api/v1/sub/coi | View current COI status |
| POST | /api/v1/sub/coi | Upload new COI |

## Document Validation

| Method | Path | Description |
|--------|------|-------------|
| POST | /api/v1/waiver-requests/{id}/validate | Trigger OCR validation (internal) |
| GET | /api/v1/waiver-requests/{id}/validation-result | Get validation result and extracted fields |

## Compliance

| Method | Path | Description |
|--------|------|-------------|
| GET | /api/v1/projects/{projectId}/compliance | COI compliance dashboard data |
| GET | /api/v1/projects/{projectId}/compliance/expiring | List subs with COIs expiring in next N days |
| POST | /api/v1/projects/{projectId}/compliance/refresh-coi | Upload COI for a sub |

## Pay Application Readiness

| Method | Path | Description |
|--------|------|-------------|
| GET | /api/v1/projects/{projectId}/pay-readiness | Pay-app readiness dashboard |
| GET | /api/v1/projects/{projectId}/pay-readiness/export | Download audit export package |

## Administration

| Method | Path | Description |
|--------|------|-------------|
| GET | /api/v1/admin/state-rules | List state waiver rules |
| PUT | /api/v1/admin/state-rules/{stateCode} | Update state waiver rule |
| GET | /api/v1/admin/audit-log | Query audit log (paginated, filterable) |

## Escalation / Follow-Up (Internal / Background)

| Trigger | Action |
|---------|--------|
| DueDate + 1 day | Send gentle reminder email to sub |
| DueDate + 3 days | Send firm follow-up email to sub |
| DueDate + 5 days | Notify GC accountant (email + in-app notification) |
| COIExpiryDate - 30 days | Send COI renewal reminder to sub |
