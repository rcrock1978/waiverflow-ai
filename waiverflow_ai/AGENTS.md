# WaiverFlow AI

**Status**: Pre-implementation — no application code exists yet. This repo is a **Spec-Driven Development (SDD)** scaffold using [Spec Kit (speckit)](https://github.com/github/spec-kit).

## SDD lifecycle (order matters)

`/speckit.specify` → `/speckit.clarify` → `/speckit.plan` → `/speckit.tasks` → `/speckit.implement`

Supporting commands: `/speckit.checklist`, `/speckit.analyze`, `/speckit.converge`, `/speckit.constitution`, `/speckit.taskstoissues`.

## Managed section — do not edit manually

The content between `<!-- SPECKIT START -->` and `<!-- SPECKIT END -->` below is auto-managed by the `agent-context` extension (runs after `specify` and `plan`). Manual edits will be overwritten.

<!-- SPECKIT START -->
This project is implementing feature **001-core-foundation** (WaiverFlow AI
core system). Read the current plan at `specs/001-core-foundation/plan.md`
for architecture, data model, contracts, and implementation details.
<!-- SPECKIT END -->

## Structure

- `.specify/` — Speckit config, templates, workflow, scripts, constitution
- `.specify/scripts/bash/` — Helper scripts (`setup-plan.sh`, `check-prerequisites.sh`, etc.)
- `specs/<NNN>-<name>/` — Feature directories (spec.md, plan.md, tasks.md, data-model.md, contracts/, checklists/)
- `AGENTS.md` — Context file; the SPECKIT block points to the active plan

## Tech stack (per constitution)

.NET 10 / C# 14 + Python 3.12 (AI services) | PostgreSQL + pgvector | Redis | Azure Service Bus / RabbitMQ | Docker + Kubernetes (AKS) | OpenID Connect | Semantic Kernel + LangChain/LlamaIndex | OpenTelemetry

## Constraints

- Never edit between `<!-- SPECKIT START -->` and `<!-- SPECKIT END -->`
- Feature numbering is sequential (from `.specify/init-options.json`)
- All speckit commands follow the same hook pattern: read `.specify/extensions.yml` for `before_<phase>`/`after_<phase>` hooks
- When invoking bash scripts, use absolute paths; escape single quotes as `'\''`
- No build/test/lint/format commands exist yet — verify after first feature implementation
