using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Subscription;
using OzoneMobileService.Application.Features.Subscription.Queries;
using OzoneMobileService.Infrastructure.Features.Subscription;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Features.Subscription.Handlers;

internal sealed class GetSubscriptionOptionsQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetSubscriptionOptionsQuery, SubscriptionOptionsResponse?>
{
    public async Task<SubscriptionOptionsResponse?> Handle(
        GetSubscriptionOptionsQuery request,
        CancellationToken cancellationToken)
    {
        var tenant = await dbContext.Tenants
            .AsNoTracking()
            .Include(t => t.SubscriptionPlan)
            .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

        if (tenant is null)
        {
            return null;
        }

        var upgradePlans = await dbContext.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.IsActive && p.TierOrder > tenant.SubscriptionPlan.TierOrder)
            .OrderBy(p => p.TierOrder)
            .ToListAsync(cancellationToken);

        var upgrades = upgradePlans.Select(SubscriptionMapper.MapPlan).ToList();

        var pending = await dbContext.SubscriptionUpgradeRequests
            .AsNoTracking()
            .Include(r => r.RequestedPlan)
            .Include(r => r.CurrentPlan)
            .Include(r => r.Tenant)
            .Include(r => r.Invoice)
            .Where(r => r.TenantId == request.TenantId && r.Status == UpgradeRequestStatuses.Pending)
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
            SubscriptionMapper.MapPlan(tenant.SubscriptionPlan),
            upgrades,
            pendingResponse);
    }
}
