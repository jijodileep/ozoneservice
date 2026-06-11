using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Branches;
using OzoneMobileService.Application.Features.Branches.Queries;

namespace OzoneMobileService.Infrastructure.Features.Branches.Handlers;

internal sealed class GetBranchQueryHandler(BranchAccessService branchAccess)
    : IRequestHandler<GetBranchQuery, BranchResponse?>
{
    public async Task<BranchResponse?> Handle(GetBranchQuery request, CancellationToken cancellationToken)
    {
        var query = await branchAccess.GetAccessibleBranchesQueryAsync(request.UserId, cancellationToken);

        var branch = await query
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.BranchId, cancellationToken);

        return branch is null ? null : BranchMapper.Map(branch);
    }
}
