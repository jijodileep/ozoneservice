using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.Platform;

namespace OzoneMobileService.Application.Interfaces;

public interface IPlatformService
{
    Task<IReadOnlyList<SubscriptionPlanResponse>> GetPlansAsync(CancellationToken cancellationToken = default);

    Task<SubscriptionPlanResponse?> CreatePlanAsync(
        CreateSubscriptionPlanRequest request,
        CancellationToken cancellationToken = default);

    Task<SubscriptionPlanResponse?> UpdatePlanAsync(
        Guid planId,
        UpdateSubscriptionPlanRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeletePlanAsync(Guid planId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShopResponse>> GetShopsAsync(CancellationToken cancellationToken = default);

    Task<PagedResult<ShopResponse>> GetShopsPagedAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default);

    Task<ShopResponse?> CreateShopAsync(CreateShopRequest request, CancellationToken cancellationToken = default);

    Task<bool> SuspendShopAsync(Guid tenantId, CancellationToken cancellationToken = default);

    Task<bool> ActivateShopAsync(Guid tenantId, CancellationToken cancellationToken = default);

    Task<bool> AssignPlanAsync(Guid tenantId, Guid planId, CancellationToken cancellationToken = default);
}
