# Security & Multi-Tenancy Checklist: Core Waiver Flow Foundation

**Purpose**: Validate completeness, clarity, and correctness of security, authentication, authorization, and tenant isolation requirements
**Created**: 2026-06-26
**Feature**: [spec.md](spec.md)

## Requirement Completeness

- [x] CHK001 - Is the authentication mechanism defined for all user roles (gc accountant, sub admin, controller)? [Completeness, Spec §Auth Note]
- [x] CHK002 - Are authorization boundaries specified between GC and subcontractor users within the same tenant? [Completeness, Spec §Auth Note]
- [x] CHK003 - Is tenant isolation enforcement specified at both API and database layers? [Completeness, Spec §Auth Note]
- [x] CHK004 - Is the scope of audit logging defined (all state changes, or only security-relevant ones)? [Completeness, Spec §FR-015]
- [x] CHK005 - Are session management and token expiry requirements documented? [Gap, Spec §Auth Note]
- [x] CHK006 - Is the process for deactivating a user and revoking access specified? [Gap, Spec §User entity]
- [x] CHK007 - Are rate limiting and abuse prevention requirements defined for public-facing endpoints (sub upload)? [Gap, Spec §FR-006]
- [x] CHK008 - Is the secure link generation mechanism for waiver requests specified (expiry, single-use, tamper-proof)? [Completeness, Spec §FR-004]

## Requirement Clarity

- [x] CHK009 - Is "limited access scoped to their company's projects" precisely defined in terms of read/write permissions? [Clarity, Spec §Auth Note]
- [x] CHK010 - Are the RBAC roles explicitly enumerated with their permitted actions? [Clarity, Spec §Auth Note]
- [x] CHK011 - Is the audit log data model specified (what fields are captured, in what format)? [Clarity, Spec §FR-015]
- [x] CHK012 - Is "tenant isolation" defined with testable criteria beyond the general statement in SC-007? [Clarity, Spec §SC-007]

## Requirement Consistency

- [x] CHK013 - Do the RBAC roles align across all user stories (GC accountant vs gc_admin vs controller)? [Consistency, Spec §US1-US5]
- [x] CHK014 - Does the sub admin persistent account model (clarified in session) align with FR-006's upload permissions? [Consistency, Spec §FR-006, Clarifications]
- [x] CHK015 - Does SC-007 (tenant isolation) have a corresponding functional requirement or is it only a success criterion? [Consistency, Spec §SC-007 vs FRs]

## Scenario Coverage

- [x] CHK016 - Are security requirements defined for a subcontractor admin attempting to access another GC's projects? [Coverage, Spec §Auth Note]
- [x] CHK017 - Are requirements defined for handling failed authentication attempts (account lockout)? [Gap]
- [x] CHK018 - Is the behavior specified when a tenant's subscription expires or is cancelled (data access, export, deletion)? [Gap]
- [x] CHK019 - Are requirements defined for securing document storage (encryption at rest, access control on blob storage)? [Gap, Spec §ComplianceDoc]

## Edge Case Coverage

- [x] CHK020 - Is the behavior specified for a user who belongs to multiple tenants (cross-tenant GC admin)? [Gap]
- [x] CHK021 - Are requirements defined for API responses that should not leak tenant existence (404 vs 403 on cross-tenant queries)? [Gap]
- [x] CHK022 - Is the audit log behavior for read-only operations on sensitive data (compliance dashboard views) specified? [Gap, Spec §FR-015]

## Non-Functional Requirements

- [x] CHK023 - Are data protection requirements (encryption in transit, at rest) explicitly documented? [Gap, Spec §Security]
- [x] CHK024 - Is the security incident detection and response process referenced or specified? [Gap]
- [x] CHK025 - Are secrets management requirements documented (API keys for email provider, OCR service)? [Gap]

## Dependencies & Assumptions

- [x] CHK026 - Is the dependency on Auth0 (or equivalent OIDC provider) documented with fallback considerations? [Assumption, Spec §Auth Note]
- [x] CHK027 - Is the assumption that "each GC organization is a separate tenant" validated against real-world multi-org scenarios? [Assumption, Spec §Auth Note]

## Notes

- This checklist validates requirements quality, not implementation correctness.
- Security requirements gaps identified here should be addressed before implementation.
