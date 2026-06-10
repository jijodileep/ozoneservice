namespace OzoneMobileService.Application.DTOs.Platform;

public sealed record SubscriptionPlanResponse(
    Guid Id,
    string Name,
    string Code,
    int MaxUsers,
    int MaxBranches,
    int MaxDevicesPerUser,
    decimal Price,
    int BillingPeriodMonths,
    int TierOrder,
    bool AllowWebLogin,
    bool AllowMobileLogin,
    bool IsActive,
    int TenantCount);
