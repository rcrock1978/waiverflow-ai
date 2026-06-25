using MediatR;

namespace WaiverFlow.DocumentRequests.Commands;

public record CreateProjectCommand(
    Guid TenantId, string Name, string? Description
) : IRequest<ProjectResponse>;

public record ProjectResponse(Guid Id, string Name, string? Description, string Status);

public record CreateSubcontractorCommand(
    Guid TenantId, Guid ProjectId, string CompanyName, string ContactName,
    string ContactEmail, string WorkState, string? Phone
) : IRequest<SubcontractorResponse>;

public record SubcontractorResponse(Guid Id, string CompanyName, string ContactEmail, string WorkState);

public record StartPayCycleCommand(
    Guid TenantId, Guid ProjectId, string Label, DateOnly DueDate
) : IRequest<List<WaiverRequestResponse>>;

public record WaiverRequestResponse(Guid Id, Guid SubcontractorId, string WaiverType, decimal Amount, string Status, DateOnly DueDate);
