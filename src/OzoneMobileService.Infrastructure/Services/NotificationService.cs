using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Notifications;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Services;

public class NotificationService(
    AppDbContext dbContext,
    ITenantContext tenantContext,
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor) : INotificationService
{
    public async Task<IReadOnlyList<NotificationResponse>> GetForCurrentUserAsync(
        CancellationToken cancellationToken = default)
    {
        var roles = await GetCurrentUserRolesAsync();
        if (roles.Count == 0)
        {
            return [];
        }

        var query = dbContext.AppNotifications.AsNoTracking().Where(n => !n.IsRead);

        if (tenantContext.IsPlatformAdmin)
        {
            query = query.Where(n => n.RoleTarget == Roles.PlatformSuperAdmin);
        }
        else if (tenantContext.HasTenant)
        {
            query = query.Where(n =>
                n.RoleTarget == Roles.TenantAdmin && n.TenantId == tenantContext.TenantId);
        }
        else
        {
            return [];
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .Select(n => new NotificationResponse(
                n.Id,
                n.TenantId,
                n.Title,
                n.Message,
                n.NotificationType,
                n.IsRead,
                n.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task MarkReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await dbContext.AppNotifications.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
        if (notification is null)
        {
            return;
        }

        notification.IsRead = true;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CheckSubscriptionExpiryAsync(CancellationToken cancellationToken = default)
    {
        var threshold = DateTime.UtcNow.AddDays(7);
        var expiringTenants = await dbContext.Tenants
            .AsNoTracking()
            .Include(t => t.SubscriptionPlan)
            .Where(t => t.IsActive && t.SubscriptionExpiresAt != null && t.SubscriptionExpiresAt <= threshold)
            .ToListAsync(cancellationToken);

        foreach (var tenant in expiringTenants)
        {
            var exists = await dbContext.AppNotifications.AnyAsync(
                n => n.TenantId == tenant.Id
                     && n.NotificationType == NotificationTypes.SubscriptionExpiring
                     && n.CreatedAt > DateTime.UtcNow.AddDays(-1),
                cancellationToken);

            if (exists)
            {
                continue;
            }

            var message =
                $"Subscription for {tenant.Name} ({tenant.SubscriptionPlan.Name}) expires on {tenant.SubscriptionExpiresAt:yyyy-MM-dd}.";

            dbContext.AppNotifications.AddRange(
                new AppNotification
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenant.Id,
                    RoleTarget = Roles.TenantAdmin,
                    Title = "Subscription expiring soon",
                    Message = message,
                    NotificationType = NotificationTypes.SubscriptionExpiring,
                    CreatedAt = DateTime.UtcNow
                },
                new AppNotification
                {
                    Id = Guid.NewGuid(),
                    RoleTarget = Roles.PlatformSuperAdmin,
                    Title = "Tenant subscription expiring",
                    Message = message,
                    NotificationType = NotificationTypes.SubscriptionExpiring,
                    CreatedAt = DateTime.UtcNow
                });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<string>> GetCurrentUserRolesAsync()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return [];
        }

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return [];
        }

        return (await userManager.GetRolesAsync(user)).ToList();
    }
}
