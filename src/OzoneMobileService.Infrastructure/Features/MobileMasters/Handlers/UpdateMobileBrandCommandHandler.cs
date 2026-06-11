using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.MobileMasters;
using OzoneMobileService.Application.Features.MobileMasters.Commands;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.MobileMasters.Handlers;

internal sealed class UpdateMobileBrandCommandHandler(AppDbContext dbContext)
    : IRequestHandler<UpdateMobileBrandCommand, MobileBrandResponse?>
{
    public async Task<MobileBrandResponse?> Handle(
        UpdateMobileBrandCommand request,
        CancellationToken cancellationToken)
    {
        var brand = await dbContext.MobileBrands
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (brand is null)
        {
            return null;
        }

        var name = MobileMasterMapper.NormalizeName(request.Name);

        if (await dbContext.MobileBrands.AnyAsync(
                b => b.Name == name && b.Id != brand.Id,
                cancellationToken))
        {
            return null;
        }

        brand.Name = name;
        brand.IsActive = request.IsActive;
        await dbContext.SaveChangesAsync(cancellationToken);
        return MobileMasterMapper.MapBrand(brand);
    }
}
