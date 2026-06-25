using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Serilog;
using WaiverFlow.Api.Auth;
using WaiverFlow.Collaboration.Services;
using WaiverFlow.Compliance.Services;
using WaiverFlow.DocumentRequests.Services;
using WaiverFlow.Reporting.Services;
using WaiverFlow.Shared.Middleware;
using WaiverFlow.Shared.Services;
using WaiverFlow.Validation.Services;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

var useDevMode = builder.Configuration.GetValue<bool>("Auth:DevMode");

if (useDevMode)
{
    builder.Services.AddAuthentication("DevMode")
        .AddScheme<AuthenticationSchemeOptions, DevModeAuthenticationHandler>("DevMode", null);
}
else
{
    builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            options.Authority = builder.Configuration["Auth:Authority"];
            options.Audience = builder.Configuration["Auth:Audience"];
            options.RequireHttpsMetadata = false;
        });
}

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("gc_accountant", p => p.RequireRole("gc_accountant"));
    options.AddPolicy("gc_admin", p => p.RequireRole("gc_admin"));
    options.AddPolicy("sub_admin", p => p.RequireRole("sub_admin"));
    options.AddPolicy("controller", p => p.RequireRole("controller"));
});

builder.Services.AddScoped<TenantContext>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IPayloadFileLogger, PayloadFileLogger>();
builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<SubcontractorService>();
builder.Services.AddScoped<SubcontractorImportService>();
builder.Services.AddScoped<WaiverRequestService>();
builder.Services.AddScoped<StateWaiverRuleService>();
builder.Services.AddScoped<COIComplianceService>();
builder.Services.AddScoped<COIReminderScheduler>();
builder.Services.AddScoped<EscalationService>();
builder.Services.AddScoped<ValidationNotificationService>();
builder.Services.AddScoped<OcrOrchestrator>();
builder.Services.AddScoped<PayReadinessService>();
builder.Services.AddScoped<AuditExportService>();
builder.Services.AddHttpClient("AiService", c => c.BaseAddress = new Uri(builder.Configuration["AiService:Url"] ?? "http://localhost:8100"));
builder.Services.AddHttpClient("SendGrid");
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        Assembly.GetExecutingAssembly(),
        typeof(WaiverFlow.DocumentRequests.Commands.CreateProjectCommand).Assembly);
});
builder.Services.AddValidatorsFromAssemblies([Assembly.GetExecutingAssembly()]);
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<TenantContextMiddleware>();
app.UseMiddleware<IdempotencyMiddleware>();
app.UseMiddleware<RateLimitMiddleware>();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

app.Run();
