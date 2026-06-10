namespace OzoneMobileService.Application.DTOs.Platform;

public sealed record CreateShopRequest(
    string Name,
    string Code,
    Guid SubscriptionPlanId,
    string DefaultBranchName,
    string ShopAdminEmail,
    string ShopAdminPassword,
    string ShopAdminDisplayName);
