using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.Features.Platform.Commands;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Platform.Handlers;

internal sealed class ActivateShopCommandHandler(AppDbContext dbContext)
    : IRequestHandler<ActivateShopCommand, bool>
{
    public async Task<bool> Handle(ActivateShopCommand command, CancellationToken cancellationToken)
    {
        var tenant = await dbContext.Tenants
            .FirstOrDefaultAsync(t => t.Id == command.TenantId, cancellationToken);
        if (tenant is null)
        {
            return false;
        }

        tenant.IsActive = true;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
