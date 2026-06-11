using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.MobileMasters;
using OzoneMobileService.Application.Features.MobileMasters.Queries;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.MobileMasters.Handlers;

internal sealed class GetMobileVariantsQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetMobileVariantsQuery, IReadOnlyList<MobileVariantResponse>>
{
    public async Task<IReadOnlyList<MobileVariantResponse>> Handle(
        GetMobileVariantsQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.MobileVariants
            .AsNoTracking()
            .Where(v => v.ModelId == request.ModelId);

        if (request.ActiveOnly)
        {
            query = query.Where(v => v.IsActive && v.Model.IsActive && v.Model.Brand.IsActive);
        }

        var variants = await query
            .OrderBy(v => v.Name)
            .ToListAsync(cancellationToken);

        return variants.Select(MobileMasterMapper.MapVariant).ToList();
    }
}
