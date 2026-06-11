using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.Exceptions;
using OzoneMobileService.Application.Features.Platform.Commands;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Platform.Handlers;

internal sealed class DeletePlanCommandHandler(AppDbContext dbContext)
    : IRequestHandler<DeletePlanCommand, bool>
{
    public async Task<bool> Handle(DeletePlanCommand command, CancellationToken cancellationToken)
    {
        var plan = await dbContext.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Id == command.PlanId, cancellationToken);
        if (plan is null)
        {
            return false;
        }

        if (await dbContext.Tenants.AnyAsync(t => t.SubscriptionPlanId == command.PlanId, cancellationToken))
        {
            throw new PlanInUseException();
        }

        dbContext.SubscriptionPlans.Remove(plan);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
