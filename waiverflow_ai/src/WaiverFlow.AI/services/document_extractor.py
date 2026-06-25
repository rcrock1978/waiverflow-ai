"""Azure AI Document Intelligence integration for document extraction."""

import structlog
from azure.ai.formrecognizer import DocumentAnalysisClient
from azure.core.credentials import AzureKeyCredential

log = structlog.get_logger()


class DocumentExtractor:
    """Extract fields from lien waiver documents using Azure AI Document Intelligence."""

    def __init__(self, endpoint: str, api_key: str):
        self.client = DocumentAnalysisClient(
            endpoint=endpoint,
            credential=AzureKeyCredential(api_key),
        )

    def extract(self, file_bytes: bytes, file_mime: str) -> dict:
        log.info("ocr.extracting", mime_type=file_mime)
        poller = self.client.begin_analyze_document("prebuilt-layout", file_bytes)
        result = poller.result()

        fields = {}
        for page in result.pages:
            for table in result.tables:
                for cell in table.cells:
                    key = f"table_{table.row_count}x{table.column_count}_{cell.row_index}_{cell.column_index}"
                    fields[key] = cell.content

        log.info("ocr.extracted", field_count=len(fields))
        return fields
