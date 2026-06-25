using WaiverFlow.DocumentRequests.Entities;

namespace WaiverFlow.Collaboration.Services;

public class EscalationService
{
    private readonly IEmailNotificationService _email;
    private readonly ILogger<EscalationService> _log;

    public EscalationService(IEmailNotificationService email, ILogger<EscalationService> log)
    {
        _email = email;
        _log = log;
    }

    public async Task ProcessOverdueAsync(List<WaiverRequest> overdue, Func<Guid, string> getSubEmail)
    {
        foreach (var request in overdue)
        {
            var daysOverdue = DateOnly.FromDateTime(DateTime.UtcNow).DayNumber - request.DueDate.DayNumber;
            var subEmail = getSubEmail(request.SubcontractorId);

            switch (daysOverdue)
            {
                case >= 5 when request.EscalationLevel < 3:
                    await _email.SendFollowUpAsync(subEmail, "Sub", "GC escalation", "Project");
                    _log.LogWarning("ESCALATION: Waiver {Id} escalated to GC level (day {Days})", request.Id, daysOverdue);
                    request.EscalationLevel = 3;
                    break;
                case >= 3 when request.EscalationLevel < 2:
                    await _email.SendFollowUpAsync(subEmail, "Sub", "firm notice", "Project");
                    request.EscalationLevel = 2;
                    break;
                case >= 1 when request.EscalationLevel < 1:
                    await _email.SendFollowUpAsync(subEmail, "Sub", "gentle reminder", "Project");
                    request.EscalationLevel = 1;
                    break;
            }
        }
    }
}
