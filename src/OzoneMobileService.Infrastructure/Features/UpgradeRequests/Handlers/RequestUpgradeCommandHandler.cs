using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Subscription;
using OzoneMobileService.Application.Exceptions;
using OzoneMobileService.Application.Features.UpgradeRequests.Commands;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Features.UpgradeRequests.Handlers;

internal sealed class RequestUpgradeCommandHandler(
    AppDbContext dbContext,
    ISubscriptionLimitService subscriptionLimitService,
    NotificationWriter notificationWriter)
    : IRequestHandler<RequestUpgradeCommand, UpgradeRequestResponse?>
{
    public async Task<UpgradeRequestResponse?> Handle(
        RequestUpgradeCommand command,
        CancellationToken cancellationToken)
    {
        var tenant = await dbContext.Tenants
            .Include(t => t.SubscriptionPlan)
            .FirstOrDefaultAsync(t => t.Id == command.TenantId, cancellationToken);

        var newPlan = await dbContext.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Id == command.PlanId && p.IsActive, cancellationToken);

        if (tenant is null || newPlan is null)
        {
            return null;
        }

        if (newPlan.TierOrder <= tenant.SubscriptionPlan.TierOrder)
        {
            throw new PlanUpgradeException();
        }

        var hasPending = await dbContext.SubscriptionUpgradeRequests
            .AnyAsync(
                r => r.TenantId == command.TenantId && r.Status == UpgradeRequestStatuses.Pending,
                cancellationToken);

        if (hasPending)
        {
            throw new InvalidOperationException("An upgrade request is already pending approval.");
        }

        await subscriptionLimitService.ValidatePlanAssignmentAsync(
            command.TenantId,
            command.PlanId,
            cancellationToken);

        var now = DateTime.UtcNow;
        var upgradeRequest = new SubscriptionUpgradeRequest
        {
            Id = Guid.NewGuid(),
            TenantId = command.TenantId,
            RequestedPlanId = newPlan.Id,
            CurrentPlanId = tenant.SubscriptionPlanId,
            Status = UpgradeRequestStatuses.Pending,
            RequestedAt = now,
            CreatedAt = now
        };

        dbContext.SubscriptionUpgradeRequests.Add(upgradeRequest);
        await dbContext.SaveChangesAsync(cancellationToken);

        await notificationWriter.CreateAsync(
            null,
            Roles.PlatformSuperAdmin,
            "Upgrade request",
            $"{tenant.Name} requested upgrade to {newPlan.Name} (₹{newPlan.Price}).",
            NotificationTypes.UpgradeRequested,
            cancellationToken);

        await notificationWriter.CreateAsync(
            tenant.Id,
            Roles.TenantAdmin,
            "Upgrade requested",
            $"Your request to upgrade to {newPlan.Name} was submitted and is awaiting approval.",
            NotificationTypes.UpgradeRequested,
            cancellationToken);

        return await GetRequestByIdAsync(upgradeRequest.Id, cancellationToken);
    }

    private async Task<UpgradeRequestResponse?> GetRequestByIdAsync(
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var item = await GetTenantUpgradeRequestsQueryHandler.LoadRequestsQuery(dbContext)
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

        return item is null ? null : UpgradeRequestMapper.Map(item);
    }
}
