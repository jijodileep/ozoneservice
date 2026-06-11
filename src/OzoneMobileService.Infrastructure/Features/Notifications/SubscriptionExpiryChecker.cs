using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Features.Notifications;

internal sealed class SubscriptionExpiryChecker(AppDbContext dbContext)
{
    internal async Task CheckAsync(CancellationToken cancellationToken = default)
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
}
