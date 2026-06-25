using System.IO.Compression;
using System.Text;
using System.Text.Json;
using WaiverFlow.DocumentRequests.Entities;

namespace WaiverFlow.Reporting.Services;

public class AuditExportService
{
    private readonly ILogger<AuditExportService> _log;

    public AuditExportService(ILogger<AuditExportService> log) => _log = log;

    public async Task<byte[]> GenerateAsync(
        string projectName, string payCycleLabel,
        List<WaiverRequest> waivers, List<object> complianceDocs)
    {
        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
        {
            var summaryCsv = new StringBuilder();
            summaryCsv.AppendLine("Subcontractor,WaiverType,Amount,Status,DueDate,ReturnedAt,ValidatedAt");
            foreach (var w in waivers)
                summaryCsv.AppendLine($"{w.SubcontractorId},{w.WaiverType},{w.Amount},{w.Status},{w.DueDate},{w.ReturnedAt?.ToString("O")},{w.ValidatedAt?.ToString("O")}");

            var summaryEntry = archive.CreateEntry($"{projectName}_{payCycleLabel}_summary.csv");
            using (var writer = new StreamWriter(summaryEntry.Open()))
                await writer.WriteAsync(summaryCsv.ToString());

            var jsonEntry = archive.CreateEntry($"{projectName}_{payCycleLabel}_metadata.json");
            using (var writer = new StreamWriter(jsonEntry.Open()))
                await writer.WriteAsync(JsonSerializer.Serialize(new
                {
                    project = projectName,
                    payCycle = payCycleLabel,
                    generatedAt = DateTime.UtcNow,
                    totalWaivers = waivers.Count,
                    completedWaivers = waivers.Count(w => w.Status is "validated" or "closed"),
                    outstandingWaivers = waivers.Count(w => w.Status is not ("validated" or "closed"))
                }, new JsonSerializerOptions { WriteIndented = true }));
        }

        _log.LogInformation("AUDIT_EXPORT: Generated package for {Project}/{Cycle} ({Count} waivers)",
            projectName, payCycleLabel, waivers.Count);

        return stream.ToArray();
    }
}
