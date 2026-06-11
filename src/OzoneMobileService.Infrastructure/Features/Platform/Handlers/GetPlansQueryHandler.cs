using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Application.Features.Platform.Queries;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Platform.Handlers;

internal sealed class GetPlansQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetPlansQuery, IReadOnlyList<SubscriptionPlanResponse>>
{
    public async Task<IReadOnlyList<SubscriptionPlanResponse>> Handle(
        GetPlansQuery request,
        CancellationToken cancellationToken)
    {
        var plans = await dbContext.SubscriptionPlans
            .AsNoTracking()
            .OrderBy(p => p.TierOrder)
            .ToListAsync(cancellationToken);

        var tenantCounts = await dbContext.Tenants
            .GroupBy(t => t.SubscriptionPlanId)
            .Select(g => new { PlanId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PlanId, x => x.Count, cancellationToken);

        return plans
            .Select(p => PlatformMapper.MapPlan(p, tenantCounts.GetValueOrDefault(p.Id)))
            .ToList();
    }
}
