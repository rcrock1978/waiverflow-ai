"""Validate extracted document fields against expected values."""

import structlog
from datetime import datetime

log = structlog.get_logger()


class DocumentValidator:
    """Validate extracted fields from lien waiver documents."""

    REQUIRED_FIELDS = ["signature", "date", "amount", "project_name"]

    def validate(self, extracted: dict, expected_amount: float) -> dict:
        errors = []

        for field in self.REQUIRED_FIELDS:
            if field not in extracted or not extracted[field]:
                errors.append(f"Missing required field: {field}")

        if "date" in extracted:
            try:
                parsed = datetime.strptime(extracted["date"], "%Y-%m-%d")
                if parsed > datetime.now():
                    errors.append("Date cannot be in the future")
            except ValueError:
                errors.append("Date format is invalid (expected YYYY-MM-DD)")

        if "amount" in extracted:
            try:
                doc_amount = float(extracted["amount"])
                if abs(doc_amount - expected_amount) > 0.01:
                    errors.append(
                        f"Amount mismatch: document says {doc_amount}, "
                        f"expected {expected_amount}"
                    )
            except ValueError:
                errors.append("Amount is not a valid number")

        is_valid = len(errors) == 0
        log.info(
            "ocr.validation",
            is_valid=is_valid,
            error_count=len(errors),
            errors=errors,
        )

        return {"valid": is_valid, "errors": errors}
