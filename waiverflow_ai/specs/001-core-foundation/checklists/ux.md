# UX Checklist: Core Waiver Flow Foundation

**Purpose**: Validate completeness, clarity, and consistency of user experience, interaction flows, and interface requirements
**Created**: 2026-06-26
**Feature**: [spec.md](spec.md)

## Requirement Completeness

- [x] CHK001 - Are dashboard views specified for all three user personas (GC accountant, GC admin, sub admin)? [Completeness, Spec §US1-US5]
- [x] CHK002 - Are notification types and delivery channels specified for each user-facing event (waiver sent, overdue, COI expiring, validation failure)? [Completeness, Spec §FR-004, FR-008, FR-010, FR-011]
- [x] CHK003 - Is the pay-cycle initiation flow specified with all steps (select subs, set deadline, review, confirm)? [Completeness, Spec §US1]
- [x] CHK004 - Are loading, empty, and error states specified for all dashboard views? [Gap, Spec §US2, US5]
- [x] CHK005 - Is the sub admin onboarding flow specified (welcome email, account setup, first login)? [Gap, Spec §FR-018]
- [x] CHK006 - Are confirmation and undo patterns specified for destructive actions (waiver override, sub removal)? [Gap, Spec §FR-014]

## Requirement Clarity

- [x] CHK007 - Is "secure link" clearly defined from a UX perspective (what the sub sees, clicks, does)? [Clarity, Spec §FR-004]
- [x] CHK008 - Is the "review before sending" step in pay-cycle creation specified with actionable UI requirements? [Clarity, Spec §US1]
- [x] CHK009 - Are the three escalation notification tiers distinct in content and tone as specified? [Clarity, Spec §US3]
- [x] CHK010 - Is "go/no-go indicator" quantified with specific visual states (green/yellow/red) and their criteria? [Clarity, Spec §US5]
- [x] CHK011 - Is the audit export interaction specified (generation time, download method, file naming)? [Clarity, Spec §FR-013]

## Requirement Consistency

- [x] CHK012 - Are notification requirements consistent across all user stories (same tone, same delivery channel for similar events)? [Consistency, Spec §US1-US5]
- [x] CHK013 - Does the sub-facing UX (document upload in US4, COI upload in US2) use consistent interaction patterns? [Consistency, Spec §US2, US4]
- [x] CHK014 - Is the compliance dashboard (US2) status display consistent with the pay-app readiness dashboard (US5)? [Consistency, Spec §US2 vs US5]
- [x] CHK015 - Does the UX account for the sub admin having multiple waiver requests across different GCs? [Consistency, Spec §FR-018 vs multi-tenant model]

## Scenario Coverage

- [x] CHK016 - Is the UX specified for a sub admin who has no pending waivers (empty state)? [Coverage, Gap]
- [x] CHK017 - Are requirements defined for a GC accountant managing projects across multiple states with different waiver rules? [Coverage, Spec §US1, FR-003]
- [x] CHK018 - Is the UX specified for a controller who only needs read-only dashboard access? [Coverage, Spec §User Roles]
- [x] CHK019 - Are mobile/tablet responsive layout requirements specified for the dashboards? [Coverage, Spec §Assumptions]

## Edge Case Coverage

- [x] CHK020 - Is the UX specified for a sub admin who clicks a secure link on an expired waiver? [Edge Case, Spec §FR-004]
- [x] CHK021 - Are requirements defined for a GC accountant who accidentally starts a duplicate pay cycle? [Gap, Spec §US1]
- [x] CHK022 - Is the UX specified for displaying a subcontractor with multiple work states on a single project? [Edge Case, Spec §Edge Cases]
- [x] CHK023 - Are requirements defined for timezone display in deadline and expiry date UIs? [Gap]

## Non-Functional Requirements

- [x] CHK024 - Are page load time targets specified for the readiness dashboard? [Gap, Spec §SC-005]
- [x] CHK025 - Are accessibility requirements (WCAG level, keyboard navigation, screen reader support) documented? [Gap]
- [x] CHK026 - Are localization requirements for date formats, currency, and state names specified for multi-state usage? [Gap, Spec §Assumptions]

## Dependencies & Assumptions

- [x] CHK027 - Is the assumption that "users have reliable internet" validated with offline/ degraded-mode requirements? [Assumption, Spec §Assumptions]
- [x] CHK028 - Is the responsive web approach (vs native mobile) documented with specific breakpoint requirements? [Assumption, Spec §Assumptions]

## Notes

- This checklist validates requirements quality, not implementation correctness.
- UX gaps identified here should be addressed before frontend implementation.
