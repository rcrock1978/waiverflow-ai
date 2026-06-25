namespace WaiverFlow.Collaboration.Services;

public class ValidationNotificationService
{
    private readonly IEmailNotificationService _email;
    private readonly ILogger<ValidationNotificationService> _log;

    public ValidationNotificationService(IEmailNotificationService email, ILogger<ValidationNotificationService> log)
    {
        _email = email;
        _log = log;
    }

    public async Task NotifyFailureAsync(string subEmail, string subName, List<string> errors)
    {
        var reason = string.Join("; ", errors);
        await _email.SendValidationFailureAsync(subEmail, subName, reason);
        _log.LogInformation("VALIDATION_FAILURE: Notified {Sub} of {Count} errors", subName, errors.Count);
    }
}
