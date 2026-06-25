using WaiverFlow.DocumentRequests.Entities;

namespace WaiverFlow.Collaboration.Services;

public class COIReminderScheduler
{
    private readonly IEmailNotificationService _email;
    private readonly ILogger<COIReminderScheduler> _log;

    public COIReminderScheduler(IEmailNotificationService email, ILogger<COIReminderScheduler> log)
    {
        _email = email;
        _log = log;
    }

    public async Task CheckAndRemindAsync(List<Subcontractor> subs, Func<Guid, string> getEmail)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        foreach (var sub in subs.Where(s => s.COIExpiryDate.HasValue))
        {
            var daysUntilExpiry = sub.COIExpiryDate!.Value.DayNumber - today.DayNumber;

            if (daysUntilExpiry is > 0 and <= 30)
            {
                var email = getEmail(sub.Id);
                await _email.SendCOIReminderAsync(email, sub.CompanyName, daysUntilExpiry);
                _log.LogInformation("COI_REMINDER: Sent to {Sub} ({Days} days until expiry)", sub.CompanyName, daysUntilExpiry);
            }
        }
    }
}
