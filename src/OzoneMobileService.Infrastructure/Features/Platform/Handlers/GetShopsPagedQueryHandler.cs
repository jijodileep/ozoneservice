using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Application.Features.Platform.Queries;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Platform.Handlers;

internal sealed class GetShopsPagedQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetShopsPagedQuery, PagedResult<ShopResponse>>
{
    public async Task<PagedResult<ShopResponse>> Handle(
        GetShopsPagedQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = dbContext.Tenants
            .AsNoTracking()
            .Include(t => t.SubscriptionPlan)
            .Include(t => t.Branches)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(t =>
                t.Name.ToLower().Contains(term) ||
                t.Code.ToLower().Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var tenants = await query
            .OrderBy(t => t.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var tenantIds = tenants.Select(t => t.Id).ToList();
        var userCounts = await dbContext.Users
            .Where(u => u.TenantId != null && tenantIds.Contains(u.TenantId.Value))
            .GroupBy(u => u.TenantId!.Value)
            .Select(g => new { TenantId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TenantId, x => x.Count, cancellationToken);

        var items = tenants
            .Select(t => PlatformMapper.MapShop(t, userCounts.GetValueOrDefault(t.Id), t.Branches.Count))
            .ToList();

        return new PagedResult<ShopResponse>(items, total, page, pageSize);
    }
}
