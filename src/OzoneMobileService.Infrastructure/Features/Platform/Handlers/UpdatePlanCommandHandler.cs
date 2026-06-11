using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Application.Features.Platform.Commands;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Platform.Handlers;

internal sealed class UpdatePlanCommandHandler(AppDbContext dbContext)
    : IRequestHandler<UpdatePlanCommand, SubscriptionPlanResponse?>
{
    public async Task<SubscriptionPlanResponse?> Handle(
        UpdatePlanCommand command,
        CancellationToken cancellationToken)
    {
        var plan = await dbContext.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Id == command.PlanId, cancellationToken);
        if (plan is null)
        {
            return null;
        }

        var request = command.Request;
        plan.Name = request.Name.Trim();
        plan.MaxUsers = request.MaxUsers;
        plan.MaxBranches = request.MaxBranches;
        plan.MaxDevicesPerUser = request.MaxDevicesPerUser;
        plan.Price = request.Price;
        plan.BillingPeriodMonths = request.BillingPeriodMonths;
        plan.TierOrder = request.TierOrder;
        plan.AllowWebLogin = request.AllowWebLogin;
        plan.AllowMobileLogin = request.AllowMobileLogin;
        plan.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);

        var tenantCount = await dbContext.Tenants
            .CountAsync(t => t.SubscriptionPlanId == command.PlanId, cancellationToken);
        return PlatformMapper.MapPlan(plan, tenantCount);
    }
}
