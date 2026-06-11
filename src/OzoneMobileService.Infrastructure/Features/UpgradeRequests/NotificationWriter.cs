using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.UpgradeRequests;

internal sealed class NotificationWriter(AppDbContext dbContext)
{
    internal async Task CreateAsync(
        Guid? tenantId,
        string roleTarget,
        string title,
        string message,
        string notificationType,
        CancellationToken cancellationToken)
    {
        dbContext.AppNotifications.Add(new AppNotification
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            RoleTarget = roleTarget,
            Title = title,
            Message = message,
            NotificationType = notificationType,
            CreatedAt = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
