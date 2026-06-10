using OzoneMobileService.Application.Interfaces;

namespace OzoneMobileService.Api.Middleware;

public class TenantMiddleware(RequestDelegate next)
{
    private static readonly PathString PlatformPrefix = new("/api/platform");
    private static readonly PathString AuthPrefix = new("/api/auth");

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

        if (context.User.Identity?.IsAuthenticated == true
            && tenantContext.IsPlatformAdmin
            && !IsPlatformAllowedPath(context.Request.Path))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { message = "Platform super admin may only access platform APIs." });
            return;
        }

        await next(context);
    }

    private static bool IsPlatformAllowedPath(PathString path) =>
        path.StartsWithSegments(PlatformPrefix)
        || path.StartsWithSegments(AuthPrefix)
        || path.StartsWithSegments("/health")
        || path.StartsWithSegments("/swagger");
}
