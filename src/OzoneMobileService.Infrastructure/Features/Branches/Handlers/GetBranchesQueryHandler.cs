using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Branches;
using OzoneMobileService.Application.Features.Branches.Queries;

namespace OzoneMobileService.Infrastructure.Features.Branches.Handlers;

internal sealed class GetBranchesQueryHandler(BranchAccessService branchAccess)
    : IRequestHandler<GetBranchesQuery, IReadOnlyList<BranchResponse>>
{
    public async Task<IReadOnlyList<BranchResponse>> Handle(
        GetBranchesQuery request,
        CancellationToken cancellationToken)
    {
        var query = await branchAccess.GetAccessibleBranchesQueryAsync(request.UserId, cancellationToken);

        var branches = await query
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);

        return branches.Select(BranchMapper.Map).ToList();
    }
}
