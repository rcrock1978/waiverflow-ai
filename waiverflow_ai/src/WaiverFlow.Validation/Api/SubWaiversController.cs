using Microsoft.AspNetCore.Mvc;
using WaiverFlow.Validation.Entities;

namespace WaiverFlow.Validation.Api;

[ApiController]
[Route("api/v1/sub/waivers")]
public class SubWaiversController : ControllerBase
{
    private readonly List<ComplianceDoc> _docs = [];

    [HttpPost("{waiverId:guid}/submit")]
    public async Task<IActionResult> Submit(Guid waiverId, IFormFile document)
    {
        var doc = new ComplianceDoc
        {
            WaiverRequestId = waiverId,
            DocType = "signed_waiver",
            BlobRef = $"waivers/{waiverId}/{document.FileName}",
            OCRStatus = "pending",
            UploadedById = Guid.Empty,
            UploadedAt = DateTime.UtcNow
        };
        _docs.Add(doc);
        return Ok(new { id = doc.Id, status = "returned", message = "Document received, OCR pending" });
    }
}
