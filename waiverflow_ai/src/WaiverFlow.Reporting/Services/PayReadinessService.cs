using WaiverFlow.DocumentRequests.Entities;
using WaiverFlow.Reporting.Entities;

namespace WaiverFlow.Reporting.Services;

public class PayReadinessService
{
    public Task<PayApplication> CalculateAsync(
        List<WaiverRequest> waivers, List<Subcontractor> subs, string payCycleLabel)
    {
        var total = waivers.Count;
        var completed = waivers.Count(w => w.Status is "validated" or "closed");
        var outstanding = total - completed;

        var allCOIValid = subs.All(s => s.COIExpiryDate.HasValue &&
            s.COIExpiryDate > DateOnly.FromDateTime(DateTime.UtcNow));

        var app = new PayApplication
        {
            PayCycleLabel = payCycleLabel,
            Status = outstanding == 0 && allCOIValid ? "ready" : "blocked",
            TotalWaiverRequests = total,
            CompletedWaiverRequests = completed,
            OutstandingWaiverRequests = outstanding,
            COIComplianceStatus = allCOIValid ? "all_valid" : "expired"
        };

        return Task.FromResult(app);
    }
}
