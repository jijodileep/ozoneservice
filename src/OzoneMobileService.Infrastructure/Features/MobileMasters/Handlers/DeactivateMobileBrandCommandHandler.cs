using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.Features.MobileMasters.Commands;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.MobileMasters.Handlers;

internal sealed class DeactivateMobileBrandCommandHandler(AppDbContext dbContext)
    : IRequestHandler<DeactivateMobileBrandCommand, bool>
{
    public async Task<bool> Handle(DeactivateMobileBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await dbContext.MobileBrands
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (brand is null)
        {
            return false;
        }

        brand.IsActive = false;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
