using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Domain.Entities;

namespace OzoneMobileService.Infrastructure.Features.Platform;

internal static class PlatformMapper
{
    internal static SubscriptionPlanResponse MapPlan(SubscriptionPlan plan, int tenantCount) =>
        new(
            plan.Id,
            plan.Name,
            plan.Code,
            plan.MaxUsers,
            plan.MaxBranches,
            plan.MaxDevicesPerUser,
            plan.Price,
            plan.BillingPeriodMonths,
            plan.TierOrder,
            plan.AllowWebLogin,
            plan.AllowMobileLogin,
            plan.IsActive,
            tenantCount);

    internal static ShopResponse MapShop(
        Tenant tenant,
        int userCount,
        int branchCount,
        SubscriptionPlan? planOverride = null)
    {
        var plan = planOverride ?? tenant.SubscriptionPlan;
        return new ShopResponse(
            tenant.Id,
            tenant.Name,
            tenant.Code,
            tenant.IsActive,
            plan.Name,
            plan.Code,
            plan.MaxUsers,
            plan.MaxBranches,
            plan.MaxDevicesPerUser,
            branchCount,
            userCount,
            tenant.SubscriptionExpiresAt,
            tenant.CreatedAt);
    }

    internal static TaxConfigurationResponse MapTaxConfiguration(TaxConfiguration config) =>
        new(
            config.Id,
            config.Name,
            config.CgstRate,
            config.SgstRate,
            config.CgstRate + config.SgstRate,
            config.IsActive,
            config.UpdatedAt);
}
