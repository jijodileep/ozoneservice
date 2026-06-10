using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Application.DTOs.Subscription;
using OzoneMobileService.Application.Exceptions;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Services;

public class SubscriptionService(
    AppDbContext dbContext,
    ISubscriptionLimitService subscriptionLimitService) : ISubscriptionService
{
    public async Task<SubscriptionOptionsResponse?> GetOptionsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var tenant = await dbContext.Tenants
            .AsNoTracking()
            .Include(t => t.SubscriptionPlan)
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

        if (tenant is null)
        {
            return null;
        }

        var upgrades = await dbContext.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.IsActive && p.TierOrder > tenant.SubscriptionPlan.TierOrder)
            .OrderBy(p => p.TierOrder)
            .Select(p => MapPlan(p))
            .ToListAsync(cancellationToken);

        return new SubscriptionOptionsResponse(
            tenant.SubscriptionPlanId,
            tenant.SubscriptionPlan.Name,
            tenant.SubscriptionPlan.TierOrder,
            tenant.SubscriptionExpiresAt,
            upgrades);
    }

    public async Task<bool> UpgradePlanAsync(
        Guid tenantId,
        Guid planId,
        CancellationToken cancellationToken = default)
    {
        var tenant = await dbContext.Tenants
            .Include(t => t.SubscriptionPlan)
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

        var newPlan = await dbContext.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Id == planId && p.IsActive, cancellationToken);

        if (tenant is null || newPlan is null)
        {
            return false;
        }

        if (newPlan.TierOrder <= tenant.SubscriptionPlan.TierOrder)
        {
            throw new PlanUpgradeException();
        }

        await subscriptionLimitService.ValidatePlanAssignmentAsync(tenantId, planId, cancellationToken);

        tenant.SubscriptionPlanId = newPlan.Id;
        tenant.SubscriptionExpiresAt = DateTime.UtcNow.AddMonths(newPlan.BillingPeriodMonths);
        await dbContext.SaveChangesAsync(cancellationToken);

        await CreateUpgradeNotificationsAsync(tenant, newPlan, cancellationToken);
        return true;
    }

    private async Task CreateUpgradeNotificationsAsync(
        Tenant tenant,
        SubscriptionPlan newPlan,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var message =
            $"{tenant.Name} upgraded to {newPlan.Name} (₹{newPlan.Price}/{newPlan.BillingPeriodMonths}mo). Expires {tenant.SubscriptionExpiresAt:yyyy-MM-dd}.";

        dbContext.AppNotifications.AddRange(
            new AppNotification
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                RoleTarget = Roles.TenantAdmin,
                Title = "Plan upgraded",
                Message = message,
                NotificationType = NotificationTypes.PlanUpgraded,
                CreatedAt = now
            },
            new AppNotification
            {
                Id = Guid.NewGuid(),
                RoleTarget = Roles.PlatformSuperAdmin,
                Title = "Tenant plan upgraded",
                Message = message,
                NotificationType = NotificationTypes.PlanUpgraded,
                CreatedAt = now
            });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static SubscriptionPlanResponse MapPlan(SubscriptionPlan plan) =>
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
