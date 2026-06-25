using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace WaiverFlow.Api.Auth;

public class DevModeAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private static readonly ClaimsPrincipal DevUser = new(new ClaimsIdentity(
    [
        new Claim(ClaimTypes.NameIdentifier, "dev-user-001"),
        new Claim(ClaimTypes.Name, "Dev Accountant"),
        new Claim(ClaimTypes.Role, "gc_accountant"),
        new Claim("tenant_id", "dev-tenant-001"),
        new Claim("scope", "openid profile email")
    ], "DevMode"));

    public DevModeAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var ticket = new AuthenticationTicket(DevUser, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
