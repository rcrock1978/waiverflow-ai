using WaiverFlow.DocumentRequests.Entities;

namespace WaiverFlow.DocumentRequests.Services;

public class ProjectService
{
    private readonly List<Project> _projects = [];

    public Task<Project> CreateAsync(Project project)
    {
        _projects.Add(project);
        return Task.FromResult(project);
    }

    public Task<List<Project>> ListAsync(Guid tenantId) =>
        Task.FromResult(_projects.Where(p => p.TenantId == tenantId).ToList());

    public Task<Project?> GetByIdAsync(Guid id) =>
        Task.FromResult(_projects.FirstOrDefault(p => p.Id == id));
}
