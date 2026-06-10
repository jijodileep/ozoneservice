using OzoneMobileService.Application.DTOs.Notifications;

namespace OzoneMobileService.Application.Interfaces;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationResponse>> GetForCurrentUserAsync(
        CancellationToken cancellationToken = default);

    Task MarkReadAsync(Guid id, CancellationToken cancellationToken = default);

    Task CheckSubscriptionExpiryAsync(CancellationToken cancellationToken = default);
}
