using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.Features.MobileMasters.Commands;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.MobileMasters.Handlers;

internal sealed class DeactivateMobileModelCommandHandler(AppDbContext dbContext)
    : IRequestHandler<DeactivateMobileModelCommand, bool>
{
    public async Task<bool> Handle(DeactivateMobileModelCommand request, CancellationToken cancellationToken)
    {
        var model = await dbContext.MobileModels
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (model is null)
        {
            return false;
        }

        model.IsActive = false;
        model.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
