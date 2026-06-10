namespace OzoneMobileService.Application.DTOs.Platform;

public sealed record ShopResponse(
    Guid Id,
    string Name,
    string Code,
    bool IsActive,
    string PlanName,
    string PlanCode,
    int MaxUsers,
    int MaxBranches,
    int MaxDevicesPerUser,
    int BranchCount,
    int UserCount,
    DateTime? SubscriptionExpiresAt,
    DateTime CreatedAt);
