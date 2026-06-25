using WaiverFlow.Shared.Entities;

namespace WaiverFlow.Reporting.Entities;

public class PayApplication : AggregateRoot
{
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public string PayCycleLabel { get; set; } = string.Empty;
    public string Status { get; set; } = "in_progress";
    public int TotalWaiverRequests { get; set; }
    public int CompletedWaiverRequests { get; set; }
    public int OutstandingWaiverRequests { get; set; }
    public string COIComplianceStatus { get; set; } = "all_valid";
    public DateTime LastCalculatedAt { get; set; } = DateTime.UtcNow;
}
