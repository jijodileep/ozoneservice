using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Branches;
using OzoneMobileService.Application.Features.Branches.Commands;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Branches.Handlers;

internal sealed class UpdateBranchCommandHandler(
    AppDbContext dbContext,
    BranchAccessService branchAccess)
    : IRequestHandler<UpdateBranchCommand, BranchResponse?>
{
    public async Task<BranchResponse?> Handle(
        UpdateBranchCommand request,
        CancellationToken cancellationToken)
    {
        if (!await branchAccess.CanManageBranchAsync(request.UserId, request.BranchId, cancellationToken))
        {
            return null;
        }

        var branch = await dbContext.Branches
            .FirstOrDefaultAsync(b => b.Id == request.BranchId, cancellationToken);

        if (branch is null)
        {
            return null;
        }

        if (!await branchAccess.IsTenantAdminAsync(request.UserId, cancellationToken) && !request.IsActive)
        {
            return null;
        }

        branch.Name = request.Name.Trim();
        branch.Address = BranchMapper.NormalizeOptional(request.Address);
        branch.Phone = BranchMapper.NormalizePhone(request.Phone);
        branch.GstNumber = BranchMapper.NormalizeGstNumber(request.GstNumber);
        branch.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);
        return BranchMapper.Map(branch);
    }
}
