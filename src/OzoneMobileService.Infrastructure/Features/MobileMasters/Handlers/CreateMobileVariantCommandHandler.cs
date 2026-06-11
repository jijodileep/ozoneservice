using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.MobileMasters;
using OzoneMobileService.Application.Features.MobileMasters.Commands;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.MobileMasters.Handlers;

internal sealed class CreateMobileVariantCommandHandler(AppDbContext dbContext)
    : IRequestHandler<CreateMobileVariantCommand, MobileVariantResponse?>
{
    public async Task<MobileVariantResponse?> Handle(
        CreateMobileVariantCommand request,
        CancellationToken cancellationToken)
    {
        var modelExists = await dbContext.MobileModels
            .AsNoTracking()
            .AnyAsync(m => m.Id == request.ModelId, cancellationToken);

        if (!modelExists)
        {
            return null;
        }

        var name = MobileMasterMapper.NormalizeName(request.Name);

        if (await dbContext.MobileVariants.AnyAsync(
                v => v.ModelId == request.ModelId && v.Name == name,
                cancellationToken))
        {
            return null;
        }

        var variant = new MobileVariant
        {
            Id = Guid.NewGuid(),
            ModelId = request.ModelId,
            Name = name,
            IsActive = true,
        };

        dbContext.MobileVariants.Add(variant);
        await dbContext.SaveChangesAsync(cancellationToken);
        return MobileMasterMapper.MapVariant(variant);
    }
}
