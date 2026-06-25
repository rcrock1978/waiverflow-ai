using MediatR;
using WaiverFlow.DocumentRequests.Entities;
using WaiverFlow.DocumentRequests.Services;

namespace WaiverFlow.DocumentRequests.Commands;

public class CreateProjectHandler(ProjectService projects)
    : IRequestHandler<CreateProjectCommand, ProjectResponse>
{
    public async Task<ProjectResponse> Handle(CreateProjectCommand cmd, CancellationToken ct)
    {
        var entity = new Project
        {
            TenantId = cmd.TenantId,
            Name = cmd.Name,
            Description = cmd.Description
        };
        var created = await projects.CreateAsync(entity);
        return new ProjectResponse(created.Id, created.Name, created.Description, created.Status);
    }
}

public class CreateSubcontractorHandler(SubcontractorService subs)
    : IRequestHandler<CreateSubcontractorCommand, SubcontractorResponse>
{
    public async Task<SubcontractorResponse> Handle(CreateSubcontractorCommand cmd, CancellationToken ct)
    {
        var entity = new Subcontractor
        {
            TenantId = cmd.TenantId,
            ProjectId = cmd.ProjectId,
            CompanyName = cmd.CompanyName,
            ContactName = cmd.ContactName,
            ContactEmail = cmd.ContactEmail,
            WorkState = cmd.WorkState,
            Phone = cmd.Phone
        };
        var created = await subs.AddAsync(entity);
        return new SubcontractorResponse(created.Id, created.CompanyName, created.ContactEmail, created.WorkState);
    }
}

public class StartPayCycleHandler(WaiverRequestService waivers, SubcontractorService subs)
    : IRequestHandler<StartPayCycleCommand, List<WaiverRequestResponse>>
{
    public async Task<List<WaiverRequestResponse>> Handle(StartPayCycleCommand cmd, CancellationToken ct)
    {
        var projectSubs = await subs.ListByProjectAsync(cmd.ProjectId);
        var subData = projectSubs.Select(s => (s.Id, s.WorkState)).ToList();
        var requests = await waivers.CreatePayCycleAsync(cmd.TenantId, cmd.ProjectId, cmd.Label, cmd.DueDate, subData);
        return requests.Select(r => new WaiverRequestResponse(r.Id, r.SubcontractorId, r.WaiverType, r.Amount, r.Status, r.DueDate)).ToList();
    }
}
