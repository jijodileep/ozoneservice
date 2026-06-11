using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.MobileMasters;
using OzoneMobileService.Application.Features.MobileMasters.Queries;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.MobileMasters.Handlers;

internal sealed class GetMobileModelsQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetMobileModelsQuery, IReadOnlyList<MobileModelResponse>>
{
    public async Task<IReadOnlyList<MobileModelResponse>> Handle(
        GetMobileModelsQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.MobileModels
            .AsNoTracking()
            .Where(m => m.BrandId == request.BrandId);

        if (request.ActiveOnly)
        {
            query = query.Where(m => m.IsActive && m.Brand.IsActive);
        }

        var models = await query
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);

        return models.Select(MobileMasterMapper.MapModel).ToList();
    }
}
