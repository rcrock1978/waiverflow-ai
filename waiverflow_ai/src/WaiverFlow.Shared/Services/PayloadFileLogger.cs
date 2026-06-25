using System.Text.Json;

namespace WaiverFlow.Shared.Services;

public interface IPayloadFileLogger
{
    Task LogCreateAsync<T>(string entityType, Guid entityId, Guid tenantId, Guid userId, T payload);
}

public class PayloadFileLogger : IPayloadFileLogger
{
    private readonly ILogger<PayloadFileLogger> _log;

    public PayloadFileLogger(ILogger<PayloadFileLogger> log) => _log = log;

    public Task LogCreateAsync<T>(string entityType, Guid entityId, Guid tenantId, Guid userId, T payload)
    {
        var date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var fileName = $"{entityType.ToLower()}_{entityId:N}-{date}.json";

        var entry = new
        {
            entityType,
            entityId,
            createdAt = DateTime.UtcNow,
            tenantId,
            createdBy = userId,
            payload
        };

        var json = JsonSerializer.Serialize(entry, new JsonSerializerOptions { WriteIndented = true });

        _log.LogInformation("PAYLOAD_LOG: {FileName} | {Json}", fileName, json);

        // In production, writes to: logs/payloads/{ServiceName}/{fileName}
        return Task.CompletedTask;
    }
}
