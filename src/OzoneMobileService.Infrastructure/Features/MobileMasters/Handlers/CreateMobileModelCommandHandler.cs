using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.MobileMasters;
using OzoneMobileService.Application.Features.MobileMasters.Commands;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.MobileMasters.Handlers;

internal sealed class CreateMobileModelCommandHandler(AppDbContext dbContext)
    : IRequestHandler<CreateMobileModelCommand, MobileModelResponse?>
{
    public async Task<MobileModelResponse?> Handle(
        CreateMobileModelCommand request,
        CancellationToken cancellationToken)
    {
        var brand = await dbContext.MobileBrands
            .AsNoTracking()
            .FirstOrDefaultAsync(
                b => b.Id == request.BrandId && b.TenantId == request.TenantId,
                cancellationToken);

        if (brand is null)
        {
            return null;
        }

        var name = MobileMasterMapper.NormalizeName(request.Name);

        if (await dbContext.MobileModels.AnyAsync(
                m => m.BrandId == request.BrandId && m.Name == name,
                cancellationToken))
        {
            return null;
        }

        var model = new MobileModel
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            BrandId = request.BrandId,
            Name = name,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.MobileModels.Add(model);
        await dbContext.SaveChangesAsync(cancellationToken);
        return MobileMasterMapper.MapModel(model);
    }
}
