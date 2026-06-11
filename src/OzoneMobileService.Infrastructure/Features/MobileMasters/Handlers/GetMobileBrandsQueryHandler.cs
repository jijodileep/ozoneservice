using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.MobileMasters;
using OzoneMobileService.Application.Features.MobileMasters.Queries;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.MobileMasters.Handlers;

internal sealed class GetMobileBrandsQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetMobileBrandsQuery, IReadOnlyList<MobileBrandResponse>>
{
    public async Task<IReadOnlyList<MobileBrandResponse>> Handle(
        GetMobileBrandsQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.MobileBrands.AsNoTracking();

        if (request.ActiveOnly)
        {
            query = query.Where(b => b.IsActive);
        }

        var brands = await query
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);

        return brands.Select(MobileMasterMapper.MapBrand).ToList();
    }
}
