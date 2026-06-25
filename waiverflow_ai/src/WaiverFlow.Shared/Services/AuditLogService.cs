using System.Text.Json;

namespace WaiverFlow.Shared.Services;

public class AuditLogEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid ActorId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public JsonDocument? Payload { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}

public interface IAuditLogService
{
    Task LogAsync(AuditLogEntry entry, CancellationToken ct = default);
}

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repo;
    private readonly ILogger<AuditLogService> _log;

    public AuditLogService(IAuditLogRepository repo, ILogger<AuditLogService> log)
    {
        _repo = repo;
        _log = log;
    }

    public async Task LogAsync(AuditLogEntry entry, CancellationToken ct = default)
    {
        await _repo.SaveAsync(entry, ct);
        _log.LogInformation(
            "AUDIT: {Action} | Entity={EntityType}/{EntityId} | Tenant={TenantId} | Actor={ActorId}",
            entry.Action, entry.EntityType, entry.EntityId, entry.TenantId, entry.ActorId);
    }
}
