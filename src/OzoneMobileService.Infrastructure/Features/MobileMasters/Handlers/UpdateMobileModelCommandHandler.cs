using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.MobileMasters;
using OzoneMobileService.Application.Features.MobileMasters.Commands;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.MobileMasters.Handlers;

internal sealed class UpdateMobileModelCommandHandler(AppDbContext dbContext)
    : IRequestHandler<UpdateMobileModelCommand, MobileModelResponse?>
{
    public async Task<MobileModelResponse?> Handle(
        UpdateMobileModelCommand request,
        CancellationToken cancellationToken)
    {
        var model = await dbContext.MobileModels
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (model is null)
        {
            return null;
        }

        var name = MobileMasterMapper.NormalizeName(request.Name);

        if (await dbContext.MobileModels.AnyAsync(
                m => m.BrandId == model.BrandId && m.Name == name && m.Id != model.Id,
                cancellationToken))
        {
            return null;
        }

        model.Name = name;
        model.IsActive = request.IsActive;
        model.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return MobileMasterMapper.MapModel(model);
    }
}
