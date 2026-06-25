namespace WaiverFlow.Shared.Services;

public class TenantContext
{
    public Guid? TenantId { get; set; }
    public Guid? UserId { get; set; }
    public string? Role { get; set; }
    public bool HasTenant => TenantId.HasValue;
}
