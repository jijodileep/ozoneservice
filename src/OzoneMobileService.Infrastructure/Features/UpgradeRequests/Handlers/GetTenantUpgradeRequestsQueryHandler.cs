using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Subscription;
using OzoneMobileService.Application.Features.UpgradeRequests.Queries;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.UpgradeRequests.Handlers;

internal sealed class GetTenantUpgradeRequestsQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetTenantUpgradeRequestsQuery, IReadOnlyList<UpgradeRequestResponse>>
{
    public async Task<IReadOnlyList<UpgradeRequestResponse>> Handle(
        GetTenantUpgradeRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await LoadRequestsQuery(dbContext)
            .Where(r => r.TenantId == request.TenantId)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync(cancellationToken);

        return items.Select(UpgradeRequestMapper.Map).ToList();
    }

    internal static IQueryable<SubscriptionUpgradeRequest> LoadRequestsQuery(AppDbContext dbContext) =>
        dbContext.SubscriptionUpgradeRequests
            .AsNoTracking()
            .Include(r => r.RequestedPlan)
            .Include(r => r.CurrentPlan)
            .Include(r => r.Tenant)
            .Include(r => r.Invoice);
}
