using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WaiverFlow.Collaboration.Services;

public interface IEmailNotificationService
{
    Task SendWaiverRequestAsync(string to, string subName, string projectName, string deadline, string link);
    Task SendFollowUpAsync(string to, string subName, string level, string projectName);
    Task SendCOIReminderAsync(string to, string subName, int daysUntilExpiry);
    Task SendValidationFailureAsync(string to, string subName, string reason);
}

public class EmailNotificationService : IEmailNotificationService
{
    private readonly HttpClient _http;
    private readonly ILogger<EmailNotificationService> _log;
    private readonly string _sendGridApiKey;
    private readonly string _fromEmail;

    public EmailNotificationService(
        IHttpClientFactory httpFactory,
        IConfiguration config,
        ILogger<EmailNotificationService> log)
    {
        _http = httpFactory.CreateClient("SendGrid");
        _log = log;
        _sendGridApiKey = config["SendGrid:ApiKey"] ?? "DEV_MODE";
        _fromEmail = config["SendGrid:FromEmail"] ?? "noreply@waiverflow.io";
    }

    public async Task SendWaiverRequestAsync(string to, string subName, string projectName, string deadline, string link)
    {
        await SendEmailAsync(to, $"Lien Waiver Request — {projectName}",
            $"Hi {subName},<br><br>A lien waiver for <b>{projectName}</b> is due {deadline}.<br>"
            + $"Please upload your signed waiver here: <a href=\"{link}\">{link}</a>");
    }

    public async Task SendFollowUpAsync(string to, string subName, string level, string projectName)
    {
        var subject = level switch
        {
            "gentle reminder" => $"Reminder: Lien Waiver for {projectName}",
            "firm notice" => $"URGENT: Overdue Lien Waiver — {projectName}",
            "GC escalation" => $"ACTION REQUIRED: Waiver Overdue — {subName}",
            _ => $"Follow-Up: {projectName}"
        };
        await SendEmailAsync(to, subject, $"Follow-up ({level}) regarding {projectName}.");
    }

    public async Task SendCOIReminderAsync(string to, string subName, int daysUntilExpiry)
    {
        await SendEmailAsync(to, $"COI Expiring Soon — {subName}",
            $"Your COI expires in {daysUntilExpiry} days. Please upload a renewed certificate.");
    }

    public async Task SendValidationFailureAsync(string to, string subName, string reason)
    {
        await SendEmailAsync(to, $"Document Validation Failed",
            $"Hi {subName},<br><br>Your document could not be validated.<br>Issues: {reason}<br>Please correct and re-upload.");
    }

    private async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        if (_sendGridApiKey == "DEV_MODE")
        {
            _log.LogInformation("EMAIL(DEV): To={To} | Subject={Subject} | Body={Body}", to, subject, htmlBody);
            return;
        }

        var payload = new
        {
            personalizations = new[] { new { to = new[] { new { email = to } } } },
            from = new { email = _fromEmail },
            subject,
            content = new[] { new { type = "text/html", value = htmlBody } }
        };

        var json = JsonSerializer.Serialize(payload);
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.sendgrid.com/v3/mail/send")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _sendGridApiKey) },
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            _log.LogError("SENDGRID: Failed to send email to {To} — {Status}", to, response.StatusCode);
        }
        else
        {
            _log.LogInformation("SENDGRID: Sent email to {To} — Subject={Subject}", to, subject);
        }
    }
}
