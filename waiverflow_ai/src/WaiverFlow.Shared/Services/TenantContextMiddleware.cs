using System.Security.Claims;

namespace WaiverFlow.Shared.Services;

public class TenantContextMiddleware
{
    private readonly RequestDelegate _next;

    public TenantContextMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, TenantContext tenantCtx)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst("tenant_id");
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            var roleClaim = context.User.FindFirst(ClaimTypes.Role);

            tenantCtx.TenantId = tenantClaim is not null ? Guid.Parse(tenantClaim.Value) : null;
            tenantCtx.UserId = userIdClaim is not null ? Guid.Parse(userIdClaim.Value) : null;
            tenantCtx.Role = roleClaim?.Value;
        }

        await _next(context);
    }
}
