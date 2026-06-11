using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Users;
using OzoneMobileService.Application.Features.Users.Queries;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Features.Branches;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Features.Users.Handlers;

internal sealed class GetUsersQueryHandler(
    AppDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    BranchAccessService branchAccess)
    : IRequestHandler<GetUsersQuery, IReadOnlyList<UserResponse>>
{
    public async Task<IReadOnlyList<UserResponse>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        if (!await branchAccess.IsTenantAdminAsync(request.UserId, cancellationToken))
        {
            return [];
        }

        var users = await dbContext.Users
            .AsNoTracking()
            .Where(u => u.TenantId == request.TenantId)
            .OrderBy(u => u.DisplayName)
            .ToListAsync(cancellationToken);

        var userIds = users.Select(u => u.Id).ToList();
        var branchRows = await dbContext.UserBranches
            .AsNoTracking()
            .Where(ub => userIds.Contains(ub.UserId))
            .Select(ub => new
            {
                ub.UserId,
                Summary = new UserBranchSummary(ub.Branch.Id, ub.Branch.Code, ub.Branch.Name)
            })
            .ToListAsync(cancellationToken);

        var branchesByUser = branchRows
            .GroupBy(x => x.UserId)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<UserBranchSummary>)g.Select(x => x.Summary).OrderBy(b => b.Name).ToList());

        var results = new List<UserResponse>(users.Count);
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            var role = ResolveDisplayRole(roles);
            branchesByUser.TryGetValue(user.Id, out var branches);
            results.Add(UserMapper.Map(user, role, branches ?? []));
        }

        return results;
    }

    private static string ResolveDisplayRole(IList<string> roles)
    {
        foreach (var assignable in TenantAssignableRoles.All)
        {
            if (roles.Contains(assignable))
            {
                return assignable;
            }
        }

        return roles.FirstOrDefault() ?? string.Empty;
    }
}
