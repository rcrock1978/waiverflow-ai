# Feature Specification: Core Waiver Flow Foundation

**Feature Directory**: `specs/001-core-foundation`

**Created**: 2026-06-26

**Status**: Draft

**Input**: WaiverFlow AI Product Requirements Document

## Clarifications

### Session 2026-06-26

- Q: How are subcontractors initially added to projects? → A: Manual onboarding with bulk import readiness
- Q: Do subcontractor admins have persistent accounts or one-time access? → A: Persistent accounts via the same OIDC identity provider

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Generate and Send Pay-Cycle Waivers (Priority: P1)

A GC project accountant starts a new pay cycle. The system identifies all
subcontractors on the project, determines which lien waiver type (conditional or
unconditional) is required for each based on the subcontractor's state, generates
the correct waiver forms, and sends them to each subcontractor with clear
instructions and a deadline. The accountant can review the list before sending.

**Why this priority**: Waiver generation is the primary trigger for the entire
document-collection workflow. Without it, nothing else happens. It delivers
immediate value by eliminating manual form preparation.

**Independent Test**: A project accountant can initiate a pay cycle for a project
with 3 subs, review the generated waivers, and confirm they were sent — all
without leaving the system.

**Acceptance Scenarios**:

1. **Given** a project with 5 active subcontractors, **When** the accountant
   starts a new pay cycle, **Then** the system generates a waiver request for
   each sub with the correct waiver type based on the sub's state.
2. **Given** a generated waiver request, **When** it is sent to the
   subcontractor, **Then** the subcontractor receives a notification with a
   secure link to view and return the signed waiver.
3. **Given** a subcontractor in Texas (conditional waiver state) and a
   subcontractor in California (unconditional waiver state), **When** waivers
   are generated, **Then** each waiver uses the type appropriate for its state.

---

### User Story 2 - Track COI Expiry and Compliance Status (Priority: P2)

A GC project accountant views a dashboard showing all subcontractors on a project
with their certificate of insurance (COI) expiry dates, current compliance
status, and upcoming expirations. The system flags subcontractors whose COIs
expire within the next 30 days and sends automated reminders.

**Why this priority**: Expired COIs create significant liability risk. Automated
tracking prevents coverage gaps without manual spreadsheet management.

**Independent Test**: An accountant can open a compliance dashboard, see which
subs have current COIs and which are expiring, and verify that reminders were
sent for upcoming expirations.

**Acceptance Scenarios**:

1. **Given** a project with 10 subcontractors, **When** the accountant views the
   compliance dashboard, **Then** each sub's COI status (valid, expiring soon,
   expired) is displayed with expiry dates.
2. **Given** a COI expiring within 30 days, **When** the expiry threshold is
   crossed, **Then** the subcontractor admin receives an automated reminder to
   provide an updated COI.
3. **Given** a subcontractor uploads a new COI, **When** it passes validation,
   **Then** the dashboard updates to show the new expiry date and the compliance
   status changes to valid.

---

### User Story 3 - Autonomous Follow-Up on Overdue Documents (Priority: P3)

When a subcontractor does not return a signed waiver by the deadline, the system
escalates through a series of automated follow-ups: a gentle reminder at 1 day
overdue, a firmer notice at 3 days, and a notification to the GC project
accountant at 5 days overdue so they can intervene directly.

**Why this priority**: Manual follow-up is the most time-consuming part of the
waiver collection process. Automating it reclaims hours per pay cycle.

**Independent Test**: A waiver request can be marked as overdue, and the system
sends the correct follow-up message at each escalation threshold without any
manual action.

**Acceptance Scenarios**:

1. **Given** a waiver request past its due date by 1 day, **When** the system
   checks overdue items, **Then** a polite reminder is sent to the subcontractor.
2. **Given** a waiver request past its due date by 3 days, **When** the system
   checks overdue items, **Then** a firmer follow-up notice is sent to the
   subcontractor.
3. **Given** a waiver request past its due date by 5 days, **When** the system
   checks overdue items, **Then** the GC project accountant is notified that
   manual intervention is needed.

---

### User Story 4 - Validate Returned Documents with OCR (Priority: P3)

When a subcontractor returns a signed waiver, the system automatically extracts
key fields (signature, date, amount, project name) and validates that the
document is complete and correctly filled. If validation fails, the system
notifies the subcontractor with specific instructions on what to correct.

**Why this priority**: Manual document review is slow and error-prone.
Automated validation catches issues instantly, reducing back-and-forth.

**Independent Test**: A signed waiver can be uploaded, and the system either
confirms it is valid or returns specific error messages about missing or
incorrect fields.

**Acceptance Scenarios**:

1. **Given** a completed and signed waiver is uploaded, **When** OCR completes,
   **Then** the system marks the document as validated and updates the waiver
   request status to received.
2. **Given** a waiver with a missing signature is uploaded, **When** OCR
   completes, **Then** the system rejects the document and notifies the
   subcontractor that a signature is required.
3. **Given** a waiver where the amount does not match the request, **When** OCR
   completes, **Then** the system flags the discrepancy for the accountant's
   review.

---

### User Story 5 - Pay-App Readiness Dashboard (Priority: P3)

A GC project accountant views a consolidated dashboard that shows, for each
project, whether all waivers and compliance documents are collected for the
current pay cycle. The dashboard provides a clear go/no-go indicator for each
project and lets the accountant export an audit package for the lender or owner.

**Why this priority**: The dashboard gives decision-makers at-a-glance
confidence that a pay application is ready to submit, preventing delays.

**Independent Test**: A project with all waivers collected shows as ready; a
project with outstanding waivers shows what is missing and why.

**Acceptance Scenarios**:

1. **Given** a project where all subs have returned valid waivers and COIs are
   current, **When** the accountant views the dashboard, **Then** the project
   shows a ready status with no outstanding items.
2. **Given** a project where 3 of 5 waivers are still outstanding, **When** the
   accountant views the dashboard, **Then** the project shows a not-ready status
   listing the 3 outstanding waivers and their due dates.
3. **Given** a project in ready status, **When** the accountant requests an
   audit export, **Then** a downloadable package is generated containing all
   waivers, COIs, and a compliance summary.

### Edge Cases

- A subcontractor operates in multiple states: each waiver uses the rules of the
  state where the work was performed.
- A subcontractor has no valid COI on file: the system blocks waiver generation
  and notifies the accountant to resolve the COI gap first.
- A waiver is returned after the pay cycle closes: the system accepts it but
  flags it for the next cycle.
- A subcontractor disputes a waiver amount: the system marks the waiver as
  disputed and notifies the accountant.
- OCR cannot read a document (poor scan quality): the system flags it for manual
  review rather than silently failing.
- A subcontractor admin has no email on file: the system notifies the accountant
  to provide contact info before sending.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow GC project accountants to initiate pay-cycle
  waiver generation for a project.
- **FR-002**: System MUST automatically identify all active subcontractors on a
  project when a pay cycle starts.
- **FR-003**: System MUST determine the correct lien waiver type (conditional or
  unconditional) per subcontractor based on the state where work was performed.
- **FR-004**: System MUST generate and deliver waiver requests to each
  subcontractor with a secure submission link and deadline.
- **FR-005**: System MUST track the status of each waiver request (pending,
  returned, validated, rejected, disputed, overdue).
- **FR-006**: System MUST allow subcontractor admins to upload signed waivers
  and certificates of insurance.
- **FR-007**: System MUST perform automated OCR validation on returned
  documents to extract and verify the following fields: signature presence,
  date (valid and non-future), waiver amount (must match request amount
  within configurable tolerance), and project name.
- **FR-008**: System MUST notify subcontractors of validation failures with
  specific correction instructions.
- **FR-009**: System MUST track COI expiry dates and flag upcoming expirations.
- **FR-010**: System MUST send automated COI renewal reminders to
  subcontractors before expiry.
- **FR-011**: System MUST escalate overdue waiver requests through automated
  follow-up steps at defined thresholds.
- **FR-012**: System MUST display a consolidated pay-app readiness dashboard per
  project showing collection status for all waivers and compliance docs.
- **FR-013**: System MUST generate an audit-ready export package (ZIP archive)
  containing all collected waivers (PDF), COI documents (PDF), and a
  compliance summary (CSV or PDF) for a given pay cycle.
- **FR-014**: System MUST allow GC project accountants to manually override or
  mark a waiver as received outside the digital workflow.
- **FR-015**: System MUST log all state-changing actions to an audit trail.
- **FR-016**: System MUST allow GC project accountants to manually add
  subcontractors to a project with name, contacts, state, and COI information.
- **FR-017**: System MUST support bulk importing subcontractors via a standard
  data file format (e.g., CSV) with validation and error reporting.
- **FR-018**: System MUST provide persistent accounts with dashboard access for
  subcontractor admins showing their active waiver requests and COI status.

**Note on authentication and multi-tenancy**: For MVP, assume each GC
organization is a separate tenant. Users authenticate within their tenant.
Subcontractor admins have persistent accounts (via the same identity provider)
with a dashboard showing their active waiver requests and COI status. Access
is scoped to their company's projects.

### Key Entities *(include if feature involves data)*

- **Project**: A construction project with associated subcontractors, pay
  cycles, and compliance requirements.
- **Subcontractor**: A company hired to perform work on a project, with
  contacts, COI records, and state-specific rules. Onboarded manually by the
  GC accountant or imported in bulk via a standard data file.
- **WaiverRequest**: A request for a signed lien waiver sent to a subcontractor
  as part of a pay cycle, with status tracking through its lifecycle.
- **ComplianceDoc**: A returned document (signed waiver, COI) that has been
  uploaded and optionally validated.
- **PayApplication**: A snapshot of waiver and compliance collection readiness
  for a given project and pay cycle.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A project accountant can generate and send waiver requests for a
  10-subcontractor project in under 5 minutes.
- **SC-002**: The system correctly determines waiver type (conditional vs
  unconditional) for all 50 US states with applicable rules.
- **SC-003**: OCR validation successfully extracts and verifies key fields from
  at least 90% of documents scanned at 200 DPI or higher with no significant
  skew or obstruction.
- **SC-004**: Automated follow-up reduces the number of waivers requiring manual
  escalation by at least 50%.
- **SC-005**: The pay-app readiness dashboard loads and displays current status
  for a project with 20 subcontractors in under 3 seconds.
- **SC-006**: A tenant administrator can onboard a new project with its
  subcontractors and begin the first pay cycle within 30 minutes of first login.
- **SC-007**: The system maintains tenant data isolation such that no user can
  see data from another tenant under any condition.
- **SC-008**: The system processes AI document validation requests such that
  the first validation result is available within 10 seconds of document
  upload under normal operating conditions.

## Assumptions

- Users have reliable internet connectivity and use a modern web browser
  (desktop-first, responsive for tablet).
- Mobile native apps are out of scope for the MVP (responsive web is sufficient).
- Each GC organization has at least one administrator who manages users and
  projects.
- Subcontractor admins will provide an email address for communication.
- The system will integrate with existing accounting or project management
  software via standard data exchange formats (list of specific integrations is
  TBD during planning).
- Email is the primary communication channel for sending waivers and reminders;
  SMS is a future enhancement.
- Waiver forms follow industry-standard templates (e.g., AIA documents) with
  state-specific modifications.
- The initial release supports English only; internationalization readiness is
  considered but not implemented.
