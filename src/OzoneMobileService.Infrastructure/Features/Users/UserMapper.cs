using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Users;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Users;

internal static class UserMapper
{
    public static UserResponse Map(ApplicationUser user, string role, IReadOnlyList<UserBranchSummary> branches) =>
        new(
            user.Id,
            user.Email ?? string.Empty,
            user.DisplayName,
            role,
            user.IsActive,
            branches);

    public static async Task<IReadOnlyList<UserBranchSummary>> LoadBranchSummariesAsync(
        AppDbContext dbContext,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.UserBranches
            .AsNoTracking()
            .Where(ub => ub.UserId == userId)
            .OrderBy(ub => ub.Branch.Name)
            .Select(ub => new UserBranchSummary(ub.Branch.Id, ub.Branch.Code, ub.Branch.Name))
            .ToListAsync(cancellationToken);
    }

    public static async Task<bool> ValidateBranchIdsAsync(
        AppDbContext dbContext,
        Guid tenantId,
        IReadOnlyList<Guid> branchIds,
        CancellationToken cancellationToken)
    {
        if (branchIds.Count == 0)
        {
            return false;
        }

        var distinctIds = branchIds.Distinct().ToList();
        var validCount = await dbContext.Branches
            .CountAsync(
                b => b.TenantId == tenantId && b.IsActive && distinctIds.Contains(b.Id),
                cancellationToken);

        return validCount == distinctIds.Count;
    }

    public static async Task SyncUserBranchesAsync(
        AppDbContext dbContext,
        Guid userId,
        IReadOnlyList<Guid> branchIds,
        CancellationToken cancellationToken)
    {
        var distinctIds = branchIds.Distinct().ToList();
        var existing = await dbContext.UserBranches
            .Where(ub => ub.UserId == userId)
            .ToListAsync(cancellationToken);

        var toRemove = existing.Where(ub => !distinctIds.Contains(ub.BranchId)).ToList();
        if (toRemove.Count > 0)
        {
            dbContext.UserBranches.RemoveRange(toRemove);
        }

        var existingIds = existing.Select(ub => ub.BranchId).ToHashSet();
        foreach (var branchId in distinctIds.Where(id => !existingIds.Contains(id)))
        {
            dbContext.UserBranches.Add(new UserBranch
            {
                UserId = userId,
                BranchId = branchId
            });
        }
    }
}
