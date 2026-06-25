# Compliance & Legal Checklist: Core Waiver Flow Foundation

**Purpose**: Validate completeness, clarity, and correctness of compliance, legal, audit, and state-specific requirements
**Created**: 2026-06-26
**Feature**: [spec.md](spec.md)

## Requirement Completeness

- [x] CHK001 - Are state-specific lien waiver type rules (conditional vs unconditional) defined for all target states? [Completeness, Spec §FR-003]
- [x] CHK002 - Is the fallback behavior specified when a subcontractor operates in a state without configured waiver rules? [Gap, Spec §FR-003]
- [x] CHK003 - Are notarization requirements per state documented in the waiver generation rules? [Completeness, Spec §Edge Cases]
- [x] CHK004 - Are COI validation criteria explicitly defined (what makes a COI acceptable vs rejected)? [Gap, Spec §US2]
- [x] CHK005 - Is the audit log retention period specified? [Gap, Spec §FR-015]
- [x] CHK006 - Are the escalation thresholds (1/3/5 days) configurable or hard-coded? [Completeness, Spec §US3]
- [x] CHK007 - Is the process for manual override of a waiver documented with compliance implications? [Clarity, Spec §FR-014]

## Requirement Clarity

- [x] CHK008 - Is the phrase "determines correct lien waiver type" quantified with specific rule sources (statute references, AIA templates)? [Clarity, Spec §FR-003]
- [x] CHK009 - Is "state-changing actions" in audit logging explicitly enumerated or scoped? [Clarity, Spec §FR-015]
- [x] CHK010 - Is "bulk importing" validation behavior defined for partial failures (some rows valid, others invalid)? [Clarity, Spec §FR-017]
- [x] CHK011 - Are COI expiry reminder timing rules unambiguous about timezone handling? [Clarity, Spec §FR-010]

## Requirement Consistency

- [x] CHK012 - Do waiver type determination rules (FR-003) align with the multi-state subcontractor edge case? [Consistency, Spec §Edge Cases]
- [x] CHK013 - Do escalation thresholds in US3 align with the WaiverRequest status state machine defined in data-model.md? [Consistency, Spec §US3 vs data-model.md]
- [x] CHK014 - Does the audit export scope (FR-013) align with the audit log scope (FR-015)? [Consistency, Spec §FR-013, FR-015]

## Scenario Coverage

- [x] CHK015 - Are compliance requirements defined for a subcontractor whose COI expires mid-pay-cycle? [Coverage, Exception Flow, Spec §US2]
- [x] CHK016 - Are requirements defined for handling disputed waiver amounts with compliance/legal impact? [Coverage, Spec §Edge Cases]
- [x] CHK017 - Are data privacy/compliance requirements specified (GDPR/CCPA implications for stored legal documents)? [Gap]
- [x] CHK018 - Are rollback or correction requirements defined when a waiver override was applied in error? [Recovery, Spec §FR-014]

## Edge Case Coverage

- [x] CHK019 - Is the behavior specified when a state changes its lien waiver laws (rule update mechanism)? [Gap, Spec §FR-003]
- [x] CHK020 - Are requirements defined for a subcontractor who is acquired/renamed mid-project? [Gap, Spec §Subcontractor entity]
- [x] CHK021 - Is the compliance impact of a tenant deletion or data export request specified? [Gap]
- [x] CHK022 - Are requirements defined for a waiver that requires notarization but is returned without one? [Edge Case, Spec §Edge Cases]

## Dependencies & Assumptions

- [x] CHK023 - Is the assumption that "waiver forms follow industry-standard templates (AIA)" validated against all target states' statutory requirements? [Assumption, Spec §Assumptions]
- [x] CHK024 - Are dependencies on external compliance data sources (state law references) documented? [Dependency, Spec §Assumptions]

## Notes

- This checklist validates requirements quality, not implementation correctness.
- Items marked incomplete require spec updates before `/speckit.plan`.
