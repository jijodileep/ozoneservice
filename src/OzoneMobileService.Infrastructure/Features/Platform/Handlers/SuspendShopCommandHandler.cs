using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.Features.Platform.Commands;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Platform.Handlers;

internal sealed class SuspendShopCommandHandler(AppDbContext dbContext)
    : IRequestHandler<SuspendShopCommand, bool>
{
    public async Task<bool> Handle(SuspendShopCommand command, CancellationToken cancellationToken)
    {
        var tenant = await dbContext.Tenants
            .FirstOrDefaultAsync(t => t.Id == command.TenantId, cancellationToken);
        if (tenant is null)
        {
            return false;
        }

        tenant.IsActive = false;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
