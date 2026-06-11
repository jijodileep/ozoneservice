using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.Subscription;
using OzoneMobileService.Application.Features.UpgradeRequests.Queries;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.UpgradeRequests.Handlers;

internal sealed class GetUpgradeRequestsPagedQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetUpgradeRequestsPagedQuery, PagedResult<UpgradeRequestResponse>>
{
    public async Task<PagedResult<UpgradeRequestResponse>> Handle(
        GetUpgradeRequestsPagedQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = GetTenantUpgradeRequestsQueryHandler.LoadRequestsQuery(dbContext)
            .IgnoreQueryFilters();

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(r => r.Status == request.Status);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(r => r.RequestedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<UpgradeRequestResponse>(
            items.Select(UpgradeRequestMapper.Map).ToList(),
            total,
            page,
            pageSize);
    }
}
