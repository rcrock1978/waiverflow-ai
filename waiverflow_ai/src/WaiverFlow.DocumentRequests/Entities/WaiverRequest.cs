using WaiverFlow.Shared.Entities;

namespace WaiverFlow.DocumentRequests.Entities;

public class WaiverRequest : AggregateRoot
{
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid SubcontractorId { get; set; }
    public string PayCycleLabel { get; set; } = string.Empty;
    public string WaiverType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly DueDate { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime? SentAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public DateTime? ValidatedAt { get; set; }
    public Guid? OverrideById { get; set; }
    public string? OverrideReason { get; set; }
    public int EscalationLevel { get; set; }
}
