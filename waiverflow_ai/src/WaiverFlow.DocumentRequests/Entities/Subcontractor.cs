using WaiverFlow.Shared.Entities;

namespace WaiverFlow.DocumentRequests.Entities;

public class Subcontractor : Entity
{
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string WorkState { get; set; } = string.Empty;
    public DateOnly? COIExpiryDate { get; set; }
    public string? COIDocumentRef { get; set; }
}
