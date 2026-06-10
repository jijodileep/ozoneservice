using OzoneMobileService.Application.DTOs.Subscription;

namespace OzoneMobileService.Application.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionOptionsResponse?> GetOptionsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<bool> UpgradePlanAsync(Guid tenantId, Guid planId, CancellationToken cancellationToken = default);
}
