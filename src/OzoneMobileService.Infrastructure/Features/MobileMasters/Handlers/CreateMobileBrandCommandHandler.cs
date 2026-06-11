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

        if (await dbContext.MobileBrands.AnyAsync(
                b => b.TenantId == request.TenantId && b.Name == name,
                cancellationToken))
        {
            return null;
        }

        var brand = new MobileBrand
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            Name = name,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.MobileBrands.Add(brand);
        await dbContext.SaveChangesAsync(cancellationToken);
        return MobileMasterMapper.MapBrand(brand);
    }
}
