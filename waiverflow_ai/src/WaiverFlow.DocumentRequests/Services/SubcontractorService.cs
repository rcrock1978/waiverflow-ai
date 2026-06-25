using WaiverFlow.DocumentRequests.Entities;

namespace WaiverFlow.DocumentRequests.Services;

public class SubcontractorService
{
    private readonly List<Subcontractor> _subs = [];

    public Task<Subcontractor> AddAsync(Subcontractor sub)
    {
        _subs.Add(sub);
        return Task.FromResult(sub);
    }

    public Task<List<Subcontractor>> ImportBulkAsync(IEnumerable<Subcontractor> subs)
    {
        var list = subs.ToList();
        _subs.AddRange(list);
        return Task.FromResult(list);
    }

    public Task<List<Subcontractor>> ListByProjectAsync(Guid projectId) =>
        Task.FromResult(_subs.Where(s => s.ProjectId == projectId).ToList());
}
