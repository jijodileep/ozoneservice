using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Application.DTOs.Subscription;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Services;

public class SubscriptionService(AppDbContext dbContext) : ISubscriptionService
{
    public Task<SubscriptionOptionsResponse?> GetOptionsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default) =>
        GetOptionsInternalAsync(tenantId, cancellationToken);

    public Task<SubscriptionOptionsResponse?> GetOptionsWithPendingAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default) =>
        GetOptionsInternalAsync(tenantId, cancellationToken);

    private async Task<SubscriptionOptionsResponse?> GetOptionsInternalAsync(
        Guid tenantId,
        CancellationToken cancellationToken)
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

        var pending = await dbContext.SubscriptionUpgradeRequests
            .AsNoTracking()
            .Include(r => r.RequestedPlan)
            .Include(r => r.CurrentPlan)
            .Include(r => r.Tenant)
            .Include(r => r.Invoice)
            .Where(r => r.TenantId == tenantId && r.Status == UpgradeRequestStatuses.Pending)
            .OrderByDescending(r => r.RequestedAt)
            .FirstOrDefaultAsync(cancellationToken);

        UpgradeRequestResponse? pendingResponse = pending is null
            ? null
            : new UpgradeRequestResponse(
                pending.Id,
                pending.TenantId,
                pending.Tenant.Name,
                pending.CurrentPlan.Name,
                pending.RequestedPlan.Name,
                pending.RequestedPlan.Price,
                pending.Status,
                pending.RequestedAt,
                pending.ReviewedAt,
                pending.RejectionReason,
                pending.InvoiceId,
                pending.Invoice?.InvoiceNumber);

        return new SubscriptionOptionsResponse(
            tenant.SubscriptionPlanId,
            tenant.SubscriptionPlan.Name,
            tenant.SubscriptionPlan.TierOrder,
            tenant.SubscriptionExpiresAt,
            upgrades,
            pendingResponse);
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
