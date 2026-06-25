using System.Text.Json;

namespace WaiverFlow.Shared.Services;

public interface IAuditLogRepository
{
    Task SaveAsync(AuditLogEntry entry, CancellationToken ct = default);
    Task<List<AuditLogEntry>> QueryAsync(Guid tenantId, int page = 1, int pageSize = 50);
}

public class AuditLogRepository : IAuditLogRepository
{
    private static readonly List<AuditLogEntry> _store = [];
    private static readonly object _lock = new();

    public Task SaveAsync(AuditLogEntry entry, CancellationToken ct = default)
    {
        lock (_lock) _store.Add(entry);
        return Task.CompletedTask;
    }

    public Task<List<AuditLogEntry>> QueryAsync(Guid tenantId, int page = 1, int pageSize = 50)
    {
        lock (_lock)
        {
            var results = _store
                .Where(e => e.TenantId == tenantId)
                .OrderByDescending(e => e.OccurredAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Task.FromResult(results);
        }
    }
}
