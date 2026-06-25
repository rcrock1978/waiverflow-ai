using WaiverFlow.Shared.Entities;

namespace WaiverFlow.DocumentRequests.Entities;

public class Project : AggregateRoot
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "active";
}
