using Microsoft.AspNetCore.Mvc;
using WaiverFlow.DocumentRequests.Services;
using WaiverFlow.Shared.Services;

namespace WaiverFlow.DocumentRequests.Api;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/subcontractors")]
public class BulkImportController : ControllerBase
{
    private readonly SubcontractorImportService _import;
    private readonly SubcontractorService _subs;
    private readonly TenantContext _tenant;

    public BulkImportController(SubcontractorImportService import, SubcontractorService subs, TenantContext tenant)
    {
        _import = import;
        _subs = subs;
        _tenant = tenant;
    }

    [HttpPost("import")]
    public async Task<IActionResult> ImportBulk(Guid projectId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "CSV file is required" });

        using var stream = file.OpenReadStream();
        var result = await _import.ImportFromCsvAsync(stream, _tenant.TenantId!.Value, projectId);

        foreach (var sub in result.Imported)
            await _subs.AddAsync(sub);

        return Ok(new
        {
            imported = result.Imported.Count,
            errors = result.Errors,
            totalErrors = result.Errors.Count
        });
    }
}
