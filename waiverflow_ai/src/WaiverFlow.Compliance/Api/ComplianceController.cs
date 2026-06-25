using WaiverFlow.Compliance.Services;
using WaiverFlow.DocumentRequests.Services;
using Microsoft.AspNetCore.Mvc;

namespace WaiverFlow.Compliance.Api;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/compliance")]
public class ComplianceController : ControllerBase
{
    private readonly SubcontractorService _subs;
    private readonly COIComplianceService _compliance;

    public ComplianceController(SubcontractorService subs, COIComplianceService compliance)
    {
        _subs = subs;
        _compliance = compliance;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard(Guid projectId)
    {
        var subs = await _subs.ListByProjectAsync(projectId);
        var statuses = await _compliance.CalculateAsync(subs);
        return Ok(statuses);
    }
}
