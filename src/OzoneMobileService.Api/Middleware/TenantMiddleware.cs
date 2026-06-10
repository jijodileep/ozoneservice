using OzoneMobileService.Application.Interfaces;

namespace OzoneMobileService.Api.Middleware;

public class TenantMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        if (context.User.Identity?.IsAuthenticated == true
            && !tenantContext.IsPlatformAdmin
            && !tenantContext.HasTenant)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { message = "Tenant context is required." });
            return;
        }

        await next(context);
    }
}
