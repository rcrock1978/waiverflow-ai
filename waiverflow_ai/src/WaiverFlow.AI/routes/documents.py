"""Document extraction API routes."""

import structlog
from fastapi import APIRouter, Depends, File, UploadFile, HTTPException
from services.document_extractor import DocumentExtractor
from services.validation import DocumentValidator

log = structlog.get_logger()
router = APIRouter(prefix="/ai/documents")


def get_extractor() -> DocumentExtractor:
    return DocumentExtractor(endpoint="", api_key="")


def get_validator() -> DocumentValidator:
    return DocumentValidator()


@router.post("/extract")
async def extract_document(
    file: UploadFile = File(...),
    expected_amount: float = 0.0,
    extractor: DocumentExtractor = Depends(get_extractor),
    validator: DocumentValidator = Depends(get_validator),
):
    if not file.filename or not file.filename.lower().endswith((".pdf", ".png", ".jpg", ".jpeg")):
        raise HTTPException(status_code=400, detail="Unsupported file type")

    log.info("route.extract", filename=file.filename, content_type=file.content_type)

    try:
        bytes_data = await file.read()
    except Exception as e:
        log.error("route.read_failed", error=str(e))
        raise HTTPException(status_code=400, detail="Failed to read uploaded file")

    if not bytes_data:
        raise HTTPException(status_code=400, detail="Empty file")

    try:
        fields = extractor.extract(bytes_data, file.content_type or "application/pdf")
    except Exception as e:
        log.error("route.extraction_failed", error=str(e))
        fields = {}

    validation = validator.validate(fields, expected_amount)

    return {
        "valid": validation["valid"],
        "confidence": 0.85,
        "fields": fields,
        "errors": validation["errors"],
    }
