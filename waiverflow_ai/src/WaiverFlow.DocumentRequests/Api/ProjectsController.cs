using MediatR;
using Microsoft.AspNetCore.Mvc;
using WaiverFlow.DocumentRequests.Commands;
using WaiverFlow.Shared.Services;

namespace WaiverFlow.DocumentRequests.Api;

[ApiController]
[Route("api/v1/projects")]
public class ProjectsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly TenantContext _tenant;

    public ProjectsController(ISender sender, TenantContext tenant)
    {
        _sender = sender;
        _tenant = tenant;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
    {
        var cmd = new CreateProjectCommand(
            _tenant.TenantId!.Value, request.Name, request.Description);
        var result = await _sender.Send(cmd);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id) => Ok(new { id, message = "Use query handler" });

    [HttpPost("{projectId:guid}/pay-cycles")]
    public async Task<IActionResult> StartPayCycle(Guid projectId,
        [FromBody] StartPayCycleRequest request)
    {
        var cmd = new StartPayCycleCommand(
            _tenant.TenantId!.Value, projectId, request.Label, request.DueDate);
        var result = await _sender.Send(cmd);
        return Ok(result);
    }

    public record CreateProjectRequest(string Name, string? Description);
    public record StartPayCycleRequest(string Label, DateOnly DueDate);
}

[ApiController]
[Route("api/v1/projects/{projectId:guid}/subcontractors")]
public class SubcontractorsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly TenantContext _tenant;

    public SubcontractorsController(ISender sender, TenantContext tenant)
    {
        _sender = sender;
        _tenant = tenant;
    }

    [HttpPost]
    public async Task<IActionResult> AddSub(Guid projectId, [FromBody] AddSubRequest request)
    {
        var cmd = new CreateSubcontractorCommand(
            _tenant.TenantId!.Value, projectId, request.CompanyName,
            request.ContactName, request.ContactEmail, request.WorkState, request.Phone);
        var result = await _sender.Send(cmd);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id) => Ok(new { id });

    public record AddSubRequest(string CompanyName, string ContactName, string ContactEmail, string WorkState, string? Phone);
}
