using WaiverFlow.DocumentRequests.Entities;

namespace WaiverFlow.DocumentRequests.Services;

public class WaiverRequestService
{
    private readonly List<WaiverRequest> _requests = [];

    public Task<List<WaiverRequest>> CreatePayCycleAsync(
        Guid tenantId, Guid projectId, string label, DateOnly dueDate,
        List<(Guid subId, string state)> subs)
    {
        var requests = subs.Select(s => new WaiverRequest
        {
            TenantId = tenantId,
            ProjectId = projectId,
            SubcontractorId = s.subId,
            PayCycleLabel = label,
            WaiverType = GetWaiverType(s.state),
            DueDate = dueDate,
            Amount = 0,
            Status = "pending"
        }).ToList();

        _requests.AddRange(requests);
        return Task.FromResult(requests);
    }

    private static string GetWaiverType(string state) => state switch
    {
        "CA" or "MA" or "MI" => "unconditional",
        "TX" or "FL" or "GA" => "conditional",
        _ => "conditional"
    };

    public Task<List<WaiverRequest>> ListByProjectAsync(Guid projectId) =>
        Task.FromResult(_requests.Where(r => r.ProjectId == projectId).ToList());

    public Task<WaiverRequest?> GetByIdAsync(Guid id) =>
        Task.FromResult(_requests.FirstOrDefault(r => r.Id == id));
}
