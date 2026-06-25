using WaiverFlow.DocumentRequests.Entities;

namespace WaiverFlow.Compliance.Services;

public class COIComplianceService
{
    public record ComplianceStatus(string CompanyName, DateOnly? ExpiryDate, string Status, int DaysUntilExpiry);

    public Task<List<ComplianceStatus>> CalculateAsync(List<Subcontractor> subs)
    {
        var results = subs.Select(s =>
        {
            var status = s.COIExpiryDate is null ? "missing" :
                s.COIExpiryDate < DateOnly.FromDateTime(DateTime.UtcNow) ? "expired" :
                s.COIExpiryDate <= DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30) ? "expiring_soon" : "valid";

            var daysUntil = s.COIExpiryDate?.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber ?? 0;

            return new ComplianceStatus(s.CompanyName, s.COIExpiryDate, status, daysUntil);
        }).ToList();

        return Task.FromResult(results);
    }
}
