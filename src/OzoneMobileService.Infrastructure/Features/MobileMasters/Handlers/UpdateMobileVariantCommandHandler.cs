using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.MobileMasters;
using OzoneMobileService.Application.Features.MobileMasters.Commands;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.MobileMasters.Handlers;

internal sealed class UpdateMobileVariantCommandHandler(AppDbContext dbContext)
    : IRequestHandler<UpdateMobileVariantCommand, MobileVariantResponse?>
{
    public async Task<MobileVariantResponse?> Handle(
        UpdateMobileVariantCommand request,
        CancellationToken cancellationToken)
    {
        var variant = await dbContext.MobileVariants
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (variant is null)
        {
            return null;
        }

        var name = MobileMasterMapper.NormalizeName(request.Name);

        if (await dbContext.MobileVariants.AnyAsync(
                v => v.ModelId == variant.ModelId && v.Name == name && v.Id != variant.Id,
                cancellationToken))
        {
            return null;
        }

        variant.Name = name;
        variant.IsActive = request.IsActive;
        variant.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return MobileMasterMapper.MapVariant(variant);
    }
}
