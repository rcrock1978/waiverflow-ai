# API & Integration Checklist: Core Waiver Flow Foundation

**Purpose**: Validate completeness, clarity, and correctness of API contracts, integration boundaries, and data exchange requirements
**Created**: 2026-06-26
**Feature**: [spec.md](spec.md)

## Requirement Completeness

- [x] CHK001 - Are error response formats specified for all API failure modes (validation errors, not found, conflict, server error)? [Completeness, Spec §Contracts]
- [x] CHK002 - Is the bulk CSV import format fully specified (required columns, data types, max file size, encoding)? [Completeness, Spec §FR-017]
- [x] CHK003 - Are OCR integration requirements specified for handling service unavailability or timeout? [Completeness, Spec §FR-007]
- [x] CHK004 - Is the email delivery service behavior specified for bounces, invalid addresses, or delivery failures? [Gap, Spec §FR-004]
- [x] CHK005 - Are webhook or callback requirements defined for notifying GC systems of events (waiver returned, validated)? [Gap, Spec §FR-013]
- [x] CHK006 - Is the audit export package format specified (file types, structure, naming convention)? [Completeness, Spec §FR-013]
- [x] CHK007 - Are idempotency key requirements defined for all command endpoints? [Gap, Spec §Contracts]

## Requirement Clarity

- [x] CHK008 - Is "standard data file format (e.g., CSV)" precisely defined with required columns, validation rules, and example? [Clarity, Spec §FR-017]
- [x] CHK009 - Are the "key fields" for OCR extraction (signature, date, amount, project name) defined with expected formats and validation rules? [Clarity, Spec §FR-007]
- [x] CHK010 - Is "secure submission link" precisely defined in terms of protocol, expiry, and access control? [Clarity, Spec §FR-004]
- [x] CHK011 - Is the pagination strategy for list endpoints specified (cursor vs offset, default page size, max page size)? [Gap, Spec §Contracts]

## Requirement Consistency

- [x] CHK012 - Do the API endpoints in contracts/api-endpoints.md align with all functional requirements in spec.md? [Consistency, Spec §FRs vs Contracts]
- [x] CHK013 - Is the OCR validation endpoint consistent with the WaiverRequest status state machine (returned → validated/rejected)? [Consistency, Spec §FR-007 vs data-model.md]
- [x] CHK014 - Do the sub-facing API endpoints (POST /api/v1/sub/waivers) enforce the same tenant isolation as GC-facing endpoints? [Consistency, Spec §Auth Note vs Contracts]

## Scenario Coverage

- [x] CHK015 - Are integration requirements defined for concurrent document submissions (two subs submitting simultaneously)? [Coverage, Spec §FR-006]
- [x] CHK016 - Are requirements defined for partial success in bulk import (some rows valid, others with errors)? [Gap, Spec §FR-017]
- [x] CHK017 - Are retry and circuit-breaker requirements defined for external service calls (OCR, email)? [Gap]
- [x] CHK018 - Is the API versioning strategy documented (URL path vs header, deprecation policy)? [Gap, Spec §Contracts]

## Edge Case Coverage

- [x] CHK019 - Is the behavior specified when OCR service returns an ambiguous result (low confidence scoring)? [Edge Case, Spec §Edge Cases]
- [x] CHK020 - Are requirements defined for handling a waiver document larger than the OCR service's max file size? [Gap, Spec §FR-007]
- [x] CHK021 - Is the behavior specified for a CSV import with duplicate subcontractor entries? [Gap, Spec §FR-017]
- [x] CHK022 - Are requirements defined for the state of in-flight API requests when a tenant is disabled mid-request? [Gap]

## Non-Functional Requirements

- [x] CHK023 - Are API rate limits specified (requests per minute per tenant, per endpoint)? [Gap, Spec §Contracts]
- [x] CHK024 - Is the file upload size limit documented for waiver and COI submissions? [Gap, Spec §FR-006]
- [x] CHK025 - Are API response time budgets specified for each endpoint family? [Gap, Spec §SC-001, SC-005]

## Dependencies & Assumptions

- [x] CHK026 - Is the dependency on Azure AI Document Intelligence documented with fallback OCR approach if unavailable? [Assumption, Spec §FR-007]
- [x] CHK027 - Is the assumption that "email is primary communication channel" validated with delivery reliability expectations? [Assumption, Spec §Assumptions]
- [x] CHK028 - Are integration points with external accounting/PM software documented even if deferred to later phases? [Dependency, Spec §Assumptions]

## Notes

- This checklist validates requirements quality, not implementation correctness.
- API contract gaps identified here should be resolved before endpoint implementation.
