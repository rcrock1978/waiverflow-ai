using Microsoft.AspNetCore.Mvc;
using WaiverFlow.DocumentRequests.Services;
using WaiverFlow.Shared.Services;

namespace WaiverFlow.DocumentRequests.Api;

[ApiController]
[Route("api/v1/waiver-requests")]
public class WaiverOverrideController : ControllerBase
{
    private readonly WaiverRequestService _waivers;
    private readonly IAuditLogService _audit;
    private readonly TenantContext _tenant;

    public WaiverOverrideController(WaiverRequestService waivers, IAuditLogService audit, TenantContext tenant)
    {
        _waivers = waivers;
        _audit = audit;
        _tenant = tenant;
    }

    [HttpPost("{id:guid}/override")]
    public async Task<IActionResult> Override(Guid id, [FromBody] OverrideRequest request)
    {
        var waiver = await _waivers.GetByIdAsync(id);
        if (waiver is null) return NotFound();

        waiver.Status = "closed";
        waiver.OverrideById = _tenant.UserId;
        waiver.OverrideReason = request.Reason;

        await _audit.LogAsync(new AuditLogEntry
        {
            TenantId = _tenant.TenantId!.Value,
            ActorId = _tenant.UserId!.Value,
            Action = "waiver.override",
            EntityType = "WaiverRequest",
            EntityId = id,
        });

        return Ok(waiver);
    }

    public record OverrideRequest(string Reason);
}
