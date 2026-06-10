using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Authorization;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Api.Middleware;

public class BranchMiddleware(RequestDelegate next)
{
    public const string BranchIdHeader = "X-Branch-Id";

    public async Task InvokeAsync(
        HttpContext context,
        ITenantContext tenantContext,
        AppDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        if (context.User.Identity?.IsAuthenticated == true && tenantContext.HasTenant)
        {
            var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? context.User.FindFirstValue("sub");

            if (userIdClaim is not null && Guid.TryParse(userIdClaim, out var userId))
            {
                var branchId = await ResolveBranchIdAsync(context, dbContext, userManager, userId);
                if (branchId.HasValue)
                {
                    var allowed = await UserCanAccessBranchAsync(
                        dbContext, userManager, userId, branchId.Value, tenantContext.TenantId!.Value);

                    if (!allowed)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(new { message = "Branch access denied." });
                        return;
                    }

                    context.Items[BranchContext.BranchIdItemKey] = branchId;
                }
            }
        }

        await next(context);
    }

    private static async Task<Guid?> ResolveBranchIdAsync(
        HttpContext context,
        AppDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        Guid userId)
    {
        if (context.Request.Headers.TryGetValue(BranchIdHeader, out var headerValue)
            && Guid.TryParse(headerValue.FirstOrDefault(), out var fromHeader))
        {
            return fromHeader;
        }

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return null;
        }

        var roles = await userManager.GetRolesAsync(user);

        if (roles.Contains(Roles.TenantAdmin))
        {
            return await dbContext.Branches
                .OrderBy(b => b.Name)
                .Select(b => (Guid?)b.Id)
                .FirstOrDefaultAsync();
        }

        return await dbContext.UserBranches
            .Where(ub => ub.UserId == userId)
            .Select(ub => (Guid?)ub.BranchId)
            .FirstOrDefaultAsync();
    }

    private static async Task<bool> UserCanAccessBranchAsync(
        AppDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        Guid userId,
        Guid branchId,
        Guid tenantId)
    {
        var branchExists = await dbContext.Branches
            .AnyAsync(b => b.Id == branchId && b.TenantId == tenantId);

        if (!branchExists)
        {
            return false;
        }

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return false;
        }

        var roles = await userManager.GetRolesAsync(user);
        if (roles.Contains(Roles.TenantAdmin))
        {
            return true;
        }

        return await dbContext.UserBranches
            .AnyAsync(ub => ub.UserId == userId && ub.BranchId == branchId);
    }
}
