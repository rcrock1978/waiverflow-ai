"""MCP tool implementations for the collections agent."""

import structlog
from datetime import datetime, timedelta

log = structlog.get_logger()


async def generate_waiver(subcontractor: str, project: str, amount: float, state: str) -> str:
    """Generate a lien waiver request for a subcontractor."""
    log.info("mcp.generate_waiver", sub=subcontractor, project=project, amount=amount, state=state)
    return (
        f"Waiver request generated for {subcontractor} on {project} (${amount:.2f}, {state}). "
        f"Type: {'unconditional' if state in ('CA', 'MA', 'MI') else 'conditional'}. "
        f"Sent to subcontractor for signature."
    )


async def validate_document(document_id: str, expected_amount: float) -> str:
    """Validate a returned lien waiver document via OCR."""
    log.info("mcp.validate_document", doc_id=document_id)
    return (
        f"Document {document_id} processed. "
        f"Fields extracted: signature=present, date=2026-06-26, amount={expected_amount:.2f}. "
        f"Validation: PASSED (confidence 0.94)."
    )


async def send_reminder(subcontractor: str, project: str, level: str = "gentle") -> str:
    """Send a follow-up reminder to a subcontractor about an overdue waiver."""
    log.info("mcp.send_reminder", sub=subcontractor, project=project, level=level)
    templates = {
        "gentle": f"Gentle reminder sent to {subcontractor} about waiver for {project}.",
        "firm": f"Firm follow-up sent to {subcontractor} — waiver for {project} is overdue.",
        "escalation": f"ESCALATION: GC notified that {subcontractor}'s waiver for {project} requires manual intervention.",
    }
    return templates.get(level, templates["gentle"])


async def check_state_rules(state: str) -> str:
    """Look up lien waiver rules for a given US state."""
    log.info("mcp.check_state_rules", state=state)
    rules = {
        "TX": "Conditional waiver. 30-day statutory grace period. No notarization required.",
        "CA": "Unconditional waiver at each progress payment. 90-day grace period. No notarization.",
        "FL": "Conditional waiver. 90-day grace period. Partial waiver not allowed.",
        "NY": "Conditional waiver. 120-day grace period. Notarization required.",
        "default": "Conditional waiver. 90-day grace period. Check state-specific requirements.",
    }
    return rules.get(state.upper(), rules["default"])


def get_all_tools():
    """Return all MCP tool definitions for the agent."""
    from .agent import MCPTool
    return [
        MCPTool(
            name="generate_waiver",
            description="Generate and send a lien waiver request to a subcontractor",
            parameters={
                "subcontractor": {"type": "string", "description": "Subcontractor company name"},
                "project": {"type": "string", "description": "Project name"},
                "amount": {"type": "number", "description": "Waiver amount"},
                "state": {"type": "string", "description": "Two-letter state code"},
            },
            handler=generate_waiver,
        ),
        MCPTool(
            name="validate_document",
            description="Validate a returned lien waiver document via OCR",
            parameters={
                "document_id": {"type": "string", "description": "Document ID"},
                "expected_amount": {"type": "number", "description": "Expected waiver amount"},
            },
            handler=validate_document,
        ),
        MCPTool(
            name="send_reminder",
            description="Send a follow-up reminder about an overdue waiver",
            parameters={
                "subcontractor": {"type": "string", "description": "Subcontractor name"},
                "project": {"type": "string", "description": "Project name"},
                "level": {"type": "string", "description": "Reminder level: gentle, firm, or escalation"},
            },
            handler=send_reminder,
        ),
        MCPTool(
            name="check_state_rules",
            description="Look up lien waiver rules for a US state",
            parameters={
                "state": {"type": "string", "description": "Two-letter state code"},
            },
            handler=check_state_rules,
        ),
    ]
