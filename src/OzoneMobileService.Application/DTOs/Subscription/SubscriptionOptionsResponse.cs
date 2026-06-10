using OzoneMobileService.Application.DTOs.Platform;

namespace OzoneMobileService.Application.DTOs.Subscription;

public sealed record SubscriptionOptionsResponse(
    Guid CurrentPlanId,
    string CurrentPlanName,
    int CurrentTierOrder,
    DateTime? SubscriptionExpiresAt,
    IReadOnlyList<SubscriptionPlanResponse> UpgradeOptions);
