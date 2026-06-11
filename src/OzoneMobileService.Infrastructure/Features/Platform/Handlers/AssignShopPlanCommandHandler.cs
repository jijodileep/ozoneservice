using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.Features.Platform.Commands;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Platform.Handlers;

internal sealed class AssignShopPlanCommandHandler(
    AppDbContext dbContext,
    ISubscriptionLimitService subscriptionLimitService)
    : IRequestHandler<AssignShopPlanCommand, bool>
{
    public async Task<bool> Handle(AssignShopPlanCommand command, CancellationToken cancellationToken)
    {
        var tenant = await dbContext.Tenants
            .Include(t => t.SubscriptionPlan)
            .FirstOrDefaultAsync(t => t.Id == command.TenantId, cancellationToken);
        var plan = await dbContext.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Id == command.PlanId && p.IsActive, cancellationToken);

        if (tenant is null || plan is null)
        {
            return false;
        }

        await subscriptionLimitService.ValidatePlanAssignmentAsync(
            command.TenantId,
            command.PlanId,
            cancellationToken);

        tenant.SubscriptionPlanId = plan.Id;
        tenant.SubscriptionExpiresAt = DateTime.UtcNow.AddMonths(plan.BillingPeriodMonths);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
