using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Application.Features.Platform.Commands;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Platform.Handlers;

internal sealed class CreatePlanCommandHandler(AppDbContext dbContext)
    : IRequestHandler<CreatePlanCommand, SubscriptionPlanResponse?>
{
    public async Task<SubscriptionPlanResponse?> Handle(
        CreatePlanCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var code = request.Code.Trim().ToUpperInvariant();
        if (await dbContext.SubscriptionPlans.AnyAsync(p => p.Code == code, cancellationToken))
        {
            return null;
        }

        var plan = new SubscriptionPlan
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Code = code,
            MaxUsers = request.MaxUsers,
            MaxBranches = request.MaxBranches,
            MaxDevicesPerUser = request.MaxDevicesPerUser,
            Price = request.Price,
            BillingPeriodMonths = request.BillingPeriodMonths,
            TierOrder = request.TierOrder,
            AllowWebLogin = request.AllowWebLogin,
            AllowMobileLogin = request.AllowMobileLogin,
            IsActive = true
        };

        dbContext.SubscriptionPlans.Add(plan);
        await dbContext.SaveChangesAsync(cancellationToken);
        return PlatformMapper.MapPlan(plan, 0);
    }
}
