using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Features.Branches;

internal sealed class BranchAccessService(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
{
    internal async Task<IQueryable<Branch>> GetAccessibleBranchesQueryAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (await IsTenantAdminAsync(userId, cancellationToken))
        {
            return dbContext.Branches;
        }

        var branchIds = dbContext.UserBranches
            .Where(ub => ub.UserId == userId)
            .Select(ub => ub.BranchId);

        return dbContext.Branches.Where(b => branchIds.Contains(b.Id));
    }

    internal async Task<bool> CanManageBranchAsync(
        Guid userId,
        Guid branchId,
        CancellationToken cancellationToken)
    {
        if (await IsTenantAdminAsync(userId, cancellationToken))
        {
            return await dbContext.Branches.AnyAsync(b => b.Id == branchId, cancellationToken);
        }

        return await dbContext.UserBranches
            .AnyAsync(ub => ub.UserId == userId && ub.BranchId == branchId, cancellationToken);
    }

    internal async Task<bool> IsTenantAdminAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return false;
        }

        var roles = await userManager.GetRolesAsync(user);
        return roles.Contains(Roles.TenantAdmin);
    }
}
