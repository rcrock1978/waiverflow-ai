using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using WaiverFlow.DocumentRequests.Entities;

namespace WaiverFlow.DocumentRequests.Services;

public class CsvImportResult
{
    public List<Subcontractor> Imported { get; set; } = [];
    public List<CsvImportError> Errors { get; set; } = [];
}

public class CsvImportError
{
    public int Row { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class SubcontractorCsvRecord
{
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string WorkState { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? COIExpiryDate { get; set; }
}

public class SubcontractorImportService
{
    private readonly ILogger<SubcontractorImportService> _log;

    public SubcontractorImportService(ILogger<SubcontractorImportService> log) => _log = log;

    public async Task<CsvImportResult> ImportFromCsvAsync(
        Stream csvStream, Guid tenantId, Guid projectId)
    {
        var result = new CsvImportResult();

        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null
        });

        int row = 1;
        await foreach (var record in csv.GetRecordsAsync<SubcontractorCsvRecord>())
        {
            row++;
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(record.CompanyName))
                errors.Add("CompanyName is required");
            if (string.IsNullOrWhiteSpace(record.ContactEmail))
                errors.Add("ContactEmail is required");
            if (string.IsNullOrWhiteSpace(record.WorkState) || record.WorkState.Length != 2)
                errors.Add("WorkState must be a valid 2-letter state code");

            if (errors.Count > 0)
            {
                result.Errors.Add(new CsvImportError { Row = row, Message = string.Join("; ", errors) });
                continue;
            }

            result.Imported.Add(new Subcontractor
            {
                TenantId = tenantId,
                ProjectId = projectId,
                CompanyName = record.CompanyName,
                ContactName = record.ContactName,
                ContactEmail = record.ContactEmail,
                WorkState = record.WorkState.ToUpper(),
                Phone = record.Phone,
                COIExpiryDate = DateOnly.TryParse(record.COIExpiryDate, out var expiry) ? expiry : null
            });
        }

        _log.LogInformation("CSV_IMPORT: {Imported} subs imported, {Errors} errors for project {Project}",
            result.Imported.Count, result.Errors.Count, projectId);

        return result;
    }
}
