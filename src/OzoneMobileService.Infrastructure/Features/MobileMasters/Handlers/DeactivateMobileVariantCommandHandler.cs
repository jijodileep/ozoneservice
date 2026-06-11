using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.Features.MobileMasters.Commands;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.MobileMasters.Handlers;

internal sealed class DeactivateMobileVariantCommandHandler(AppDbContext dbContext)
    : IRequestHandler<DeactivateMobileVariantCommand, bool>
{
    public async Task<bool> Handle(DeactivateMobileVariantCommand request, CancellationToken cancellationToken)
    {
        var variant = await dbContext.MobileVariants
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (variant is null)
        {
            return false;
        }

        variant.IsActive = false;
        variant.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
