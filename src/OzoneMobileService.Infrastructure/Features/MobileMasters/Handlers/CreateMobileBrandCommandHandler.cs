using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.MobileMasters;
using OzoneMobileService.Application.Features.MobileMasters.Commands;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.MobileMasters.Handlers;

internal sealed class CreateMobileBrandCommandHandler(AppDbContext dbContext)
    : IRequestHandler<CreateMobileBrandCommand, MobileBrandResponse?>
{
    public async Task<MobileBrandResponse?> Handle(
        CreateMobileBrandCommand request,
        CancellationToken cancellationToken)
    {
        var name = MobileMasterMapper.NormalizeName(request.Name);

        if (await dbContext.MobileBrands.AnyAsync(b => b.Name == name, cancellationToken))
        {
            return null;
        }

        var brand = new MobileBrand
        {
            Id = Guid.NewGuid(),
            Name = name,
            IsActive = true,
        };

        dbContext.MobileBrands.Add(brand);
        await dbContext.SaveChangesAsync(cancellationToken);
        return MobileMasterMapper.MapBrand(brand);
    }
}
