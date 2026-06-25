using Microsoft.AspNetCore.Mvc;
using WaiverFlow.DocumentRequests.Services;
using WaiverFlow.Reporting.Services;
using WaiverFlow.Shared.Services;

namespace WaiverFlow.Reporting.Api;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/pay-readiness")]
public class PayReadinessController : ControllerBase
{
    private readonly PayReadinessService _readiness;
    private readonly AuditExportService _export;
    private readonly WaiverRequestService _waivers;
    private readonly SubcontractorService _subs;
    private readonly TenantContext _tenant;

    public PayReadinessController(
        PayReadinessService readiness, AuditExportService export,
        WaiverRequestService waivers, SubcontractorService subs, TenantContext tenant)
    {
        _readiness = readiness;
        _export = export;
        _waivers = waivers;
        _subs = subs;
        _tenant = tenant;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard(Guid projectId)
    {
        var allWaivers = await _waivers.ListByProjectAsync(projectId);
        var allSubs = await _subs.ListByProjectAsync(projectId);
        var result = await _readiness.CalculateAsync(allWaivers, allSubs, "current");
        return Ok(result);
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(Guid projectId)
    {
        var allWaivers = await _waivers.ListByProjectAsync(projectId);
        var zipBytes = await _export.GenerateAsync(
            $"Project-{projectId:N}", "current", allWaivers, []);
        return File(zipBytes, "application/zip", $"audit-{projectId:N}.zip");
    }
}
