using WaiverFlow.Shared.Entities;

namespace WaiverFlow.Validation.Entities;

public class ComplianceDoc : AggregateRoot
{
    public Guid TenantId { get; set; }
    public Guid? WaiverRequestId { get; set; }
    public Guid SubcontractorId { get; set; }
    public string DocType { get; set; } = string.Empty;
    public string? BlobRef { get; set; }
    public string OCRStatus { get; set; } = "pending";
    public double? OCRConfidence { get; set; }
    public string? ExtractedFields { get; set; }
    public string? ValidationErrors { get; set; }
    public Guid UploadedById { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
