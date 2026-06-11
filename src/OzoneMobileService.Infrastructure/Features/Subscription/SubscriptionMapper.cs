using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Domain.Entities;

namespace OzoneMobileService.Infrastructure.Features.Subscription;

internal static class SubscriptionMapper
{
    internal static SubscriptionPlanResponse MapPlan(SubscriptionPlan plan) =>
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
            0);
}
