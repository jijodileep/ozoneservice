namespace OzoneMobileService.Application.DTOs.Platform;

public sealed record UpdateSubscriptionPlanRequest(
    string Name,
    int MaxUsers,
    int MaxBranches,
    int MaxDevicesPerUser,
    decimal Price,
    int BillingPeriodMonths,
    int TierOrder,
    bool AllowWebLogin,
    bool AllowMobileLogin,
    bool IsActive);
