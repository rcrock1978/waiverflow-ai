# Data Model: Core Waiver Flow Foundation

## Entities

### Tenant
| Field | Type | Notes |
|-------|------|-------|
| Id | Guid | Primary key |
| Name | string | GC organization name |
| Slug | string | URL-safe unique identifier |
| CreatedAt | DateTime | |
| UpdatedAt | DateTime | |

### User
| Field | Type | Notes |
|-------|------|-------|
| Id | Guid | Primary key |
| TenantId | Guid | FK → Tenant; drives RLS |
| ExternalId | string | OIDC provider subject ID |
| Email | string | |
| DisplayName | string | |
| Role | enum | gc_accountant, gc_admin, sub_admin, controller |
| IsActive | bool | Soft-disable for access revocation |
| CreatedAt | DateTime | |
| UpdatedAt | DateTime | |

**Constraints**: Unique (TenantId, Email). Unique (ExternalId) across the system.

### Project
| Field | Type | Notes |
|-------|------|-------|
| Id | Guid | Primary key |
| TenantId | Guid | FK → Tenant; RLS key |
| Name | string | |
| Description | string | Optional |
| Status | enum | active, completed, on_hold |
| CreatedAt | DateTime | |
| UpdatedAt | DateTime | |

### Subcontractor
| Field | Type | Notes |
|-------|------|-------|
| Id | Guid | Primary key |
| TenantId | Guid | FK → Tenant; RLS key |
| ProjectId | Guid | FK → Project |
| CompanyName | string | |
| ContactName | string | Primary contact |
| ContactEmail | string | For notifications and login |
| Phone | string | Optional |
| WorkState | string | State where work is performed; drives waiver type |
| COIExpiryDate | date | Nullable; tracked for compliance |
| COIDocumentRef | string | Blob storage reference to uploaded COI |
| CreatedAt | DateTime | |
| UpdatedAt | DateTime | |

**Constraints**: Unique (TenantId, ProjectId, CompanyName).

**Validation**: If COIExpiryDate is null, block waiver generation (FR-002 edge case).

### StateWaiverRule
| Field | Type | Notes |
|-------|------|-------|
| Id | Guid | Primary key |
| StateCode | string | 2-letter US state code |
| WaiverType | enum | conditional, unconditional, both |
| AllowsPartialWaiver | bool | |
| RequiresNotarization | bool | |
| StatutoryGracePeriodDays | int | Days after work before lien rights expire |
| Notes | string | Free-text guidance for accountants |
| UpdatedAt | DateTime | |

**Seed data**: Initial set for top 10 states; admin-maintained.

### WaiverRequest
| Field | Type | Notes |
|-------|------|-------|
| Id | Guid | Primary key |
| TenantId | Guid | FK → Tenant; RLS key |
| ProjectId | Guid | FK → Project |
| SubcontractorId | Guid | FK → Subcontractor |
| PayCycleLabel | string | e.g. "June 2026 — Cycle 1" |
| WaiverType | enum | conditional, unconditional |
| Amount | decimal | Waiver amount |
| DueDate | date | Deadline for sub to return signed waiver |
| Status | enum | See state machine below |
| SentAt | DateTime | When notification was dispatched |
| ReturnedAt | DateTime | Nullable; when document was uploaded |
| ValidatedAt | DateTime | Nullable; when OCR passed |
| OverrideById | Guid | Nullable; FK → User if accountant manually overrode |
| OverrideReason | string | Nullable |
| EscalationLevel | int | 0 = none, 1 = gentle reminder, 2 = firm notice, 3 = GC alerted |
| CreatedAt | DateTime | |
| UpdatedAt | DateTime | |

**WaiverRequest Status State Machine**:

```
pending ──► sent ──► returned ──► validated ──► closed
  │          │          │              │
  │          │          ├──► rejected ──┤
  │          │          │              │
  │          │          └──► disputed ──┘
  │          │
  │          └──► overdue ──► escalated (level 1..3)
  │
  └──► cancelled
```

- **pending**: Created but not yet sent to sub
- **sent**: Notification dispatched; waiting for sub action
- **returned**: Document uploaded; awaiting OCR validation
- **validated**: OCR passed; document accepted
- **rejected**: OCR failed; sub notified of corrections
- **disputed**: Sub disputed the amount; accountant must resolve
- **overdue**: Past DueDate; escalation sequence begins
- **escalated**: Escalation level 1-3 active
- **closed**: Final state; waived or resolved
- **cancelled**: Abandoned by accountant

### ComplianceDoc
| Field | Type | Notes |
|-------|------|-------|
| Id | Guid | Primary key |
| TenantId | Guid | FK → Tenant; RLS key |
| WaiverRequestId | Guid | Nullable; FK → WaiverRequest |
| SubcontractorId | Guid | FK → Subcontractor (for COI docs not tied to a request) |
| DocType | enum | signed_waiver, certificate_of_insurance, other |
| BlobRef | string | Storage reference |
| OCRStatus | enum | pending, completed, failed |
| OCRConfidence | float | 0.0–1.0 |
| ExtractedFields | jsonb | Key-value pairs from OCR |
| ValidationErrors | jsonb | Array of error messages if validation failed |
| UploadedById | Guid | FK → User |
| UploadedAt | DateTime | |
| CreatedAt | DateTime | |

### PayApplication
| Field | Type | Notes |
|-------|------|-------|
| Id | Guid | Primary key |
| TenantId | Guid | FK → Tenant; RLS key |
| ProjectId | Guid | FK → Project |
| PayCycleLabel | string | |
| Status | enum | in_progress, ready, blocked |
| TotalWaiverRequests | int | |
| CompletedWaiverRequests | int | Count where status = validated or closed |
| OutstandingWaiverRequests | int | |
| COIComplianceStatus | enum | all_valid, expiring_soon, expired |
| LastCalculatedAt | DateTime | When the readiness was last computed |
| CreatedAt | DateTime | |
| UpdatedAt | DateTime | |

### AuditLog
| Field | Type | Notes |
|-------|------|-------|
| Id | Guid | Primary key |
| TenantId | Guid | FK → Tenant |
| ActorId | Guid | FK → User |
| Action | string | e.g. "waiver.sent", "waiver.validated", "sub.onboarded" |
| EntityType | string | e.g. "WaiverRequest", "Subcontractor" |
| EntityId | Guid | ID of affected entity |
| Payload | jsonb | Before/after snapshots, metadata |
| OccurredAt | DateTime | |

**Constraints**: Append-only. No UPDATE or DELETE on this table.

## Relationships

```
Tenant 1──* User
Tenant 1──* Project
Tenant 1──* Subcontractor
Tenant 1──* WaiverRequest
Tenant 1──* ComplianceDoc
Tenant 1──* PayApplication
Tenant 1──* AuditLog

Project 1──* Subcontractor
Project 1──* WaiverRequest
Project 1──* PayApplication

Subcontractor 1──* WaiverRequest
Subcontractor 1──* ComplianceDoc

WaiverRequest 1──* ComplianceDoc (a request can have multiple return attempts)
```

## Concurrency & Conflict Resolution

- Optimistic concurrency via row version / timestamp on all aggregates
- Idempotency keys on all command endpoints (API layer enforces uniqueness per key)
- No concurrent edits expected on the same WaiverRequest (sole owner is the GC)
- AuditLog is append-only; no conflict possible
