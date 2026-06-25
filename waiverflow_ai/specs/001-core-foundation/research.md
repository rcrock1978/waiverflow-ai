# Research: Core Waiver Flow Foundation

## 1. State-Specific Lien Waiver Rules

**Decision**: Model state rules as a configuration table with conditional/unconditional
type mapping, statutory format requirements, and signature/notarization rules.

**Rationale**: Each state has distinct mechanics lien laws that determine whether
conditional or unconditional waivers are used, what fields are mandatory, and
whether notarization is required. A data-driven approach allows the rules to be
maintained without code changes.

**Alternatives considered**:
- Hard-coded switch statements per state — not scalable, requires deployment for
  every rule change
- External API for lien law data — no comprehensive, reliable API exists for
  all 50 states' waiver rules at reasonable cost
- AI-driven rule extraction from statutes — too unreliable for legal compliance;
  suitable as a research aid only

**Implementation notes**:
- Source: industry-standard reference (AIA documents + state statute summaries)
- Initial rule set: 10 most active construction states (CA, TX, FL, NY, IL,
  GA, NC, AZ, WA, CO); remaining states marked as requiring manual accountant
  selection until rules are added
- Update mechanism: admin UI for configuring state rules; changes are audit-logged

## 2. OCR for Document Validation

**Decision**: Use Azure AI Document Intelligence (formerly Form Recognizer) for
pre-built document extraction, with custom field validation logic in the Python
AI service.

**Rationale**: Azure AI Document Intelligence provides pre-trained models for
form recognition, field extraction, and document classification, with strong
performance on structured documents like lien waivers. The Python AI service
wraps this with custom validation logic (field comparison against known request
data, signature detection confidence, cross-field consistency checks).

**Alternatives considered**:
- Tesseract OCR — free but requires significant custom training and tuning;
  accuracy is lower on varied document formats
- AWS Textract — comparable to Azure DI; chose Azure for cloud stack consistency
- Custom ML model — excessive complexity for MVP; revisit if accuracy targets
  (SC-003: 90%) are not met with the pre-built service

**Implementation notes**:
- Document Intelligence SDK for Python handles extraction
- Custom Python service validates extracted fields against the WaiverRequest
  data (amount match, signature presence, date validity)
- Low-confidence extractions (>30% uncertainty) are flagged for manual review
- Document images are stored in Azure Blob Storage with tenant-scoped access

## 3. Email Delivery Infrastructure

**Decision**: Use SendGrid (Twilio) for transactional email delivery, wrapped
behind an Anti-Corruption Layer in the Collaboration bounded context.

**Rationale**: SendGrid provides reliable delivery, template support, open/click
tracking, and bounce handling at predictable cost for transactional volumes.
Wrapping behind an ACL allows future provider swaps without domain impact.

**Alternatives considered**:
- SMTP relay via Azure Communication Services — viable but less feature-rich
  for template management and delivery analytics
- Direct SMTP — poor deliverability, no tracking, no bounce handling
- Amazon SES — comparable to SendGrid; SendGrid has better template management UX

**Implementation notes**:
- Email templates: waiver request notification, follow-up reminders (3 tiers),
  COI expiry warning, validation failure notification, escalation alert
- Bounce handling: automatic flagging of invalid addresses, notification to GC
  accountant
- Secure links: signed, time-limited URLs for document submission portal;
  links valid until waiver deadline + 7 days

## 4. Identity Provider

**Decision**: Use Auth0 for MVP (developer-friendly, generous free tier, built-in
OIDC/OAuth2 support, social login for subs, RBAC).

**Rationale**: Auth0 provides the fastest path to production with pre-built
universal login, tenant-aware user management, and role assignment. Migration
to Entra ID is possible when enterprise SSO/SCIM requirements emerge.

**Alternatives considered**:
- Entra ID (Azure AD) — better long-term fit for enterprise; more complex setup,
  no free tier, overkill for MVP tenant volumes
- Keycloak — self-hosted, free, but adds operational overhead; no dedicated
  team to manage it
- Custom auth — explicitly prohibited by constitution (MUST use OIDC)

**Implementation notes**:
- Multi-tenant Auth0 configuration: one Auth0 tenant per GC org, or use
  organizations feature for multi-tenancy within a single Auth0 tenant
- Roles: `gc_accountant`, `gc_admin`, `sub_admin`, `controller`
- User metadata stores tenant ID for row-level security enforcement
- SCIM provisioning deferred to Phase 2 (enterprise readiness)
