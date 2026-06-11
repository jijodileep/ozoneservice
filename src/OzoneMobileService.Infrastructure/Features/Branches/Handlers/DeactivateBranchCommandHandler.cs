using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.Features.Branches.Commands;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Branches.Handlers;

internal sealed class DeactivateBranchCommandHandler(
    AppDbContext dbContext,
    BranchAccessService branchAccess)
    : IRequestHandler<DeactivateBranchCommand, bool>
{
    public async Task<bool> Handle(DeactivateBranchCommand request, CancellationToken cancellationToken)
    {
        if (!await branchAccess.IsTenantAdminAsync(request.UserId, cancellationToken))
        {
            return false;
        }

        var branch = await dbContext.Branches
            .FirstOrDefaultAsync(b => b.Id == request.BranchId, cancellationToken);

        if (branch is null)
        {
            return false;
        }

        branch.IsActive = false;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
