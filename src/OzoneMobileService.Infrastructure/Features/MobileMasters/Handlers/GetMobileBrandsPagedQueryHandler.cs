using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.MobileMasters;
using OzoneMobileService.Application.Features.MobileMasters.Queries;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.MobileMasters.Handlers;

internal sealed class GetMobileBrandsPagedQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetMobileBrandsPagedQuery, PagedResult<MobileBrandResponse>>
{
    public async Task<PagedResult<MobileBrandResponse>> Handle(
        GetMobileBrandsPagedQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = dbContext.MobileBrands.AsNoTracking();

        if (request.ActiveOnly)
        {
            query = query.Where(b => b.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(b => b.Name.ToLower().Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var brands = await query
            .OrderBy(b => b.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<MobileBrandResponse>(
            brands.Select(MobileMasterMapper.MapBrand).ToList(),
            total,
            page,
            pageSize);
    }
}
