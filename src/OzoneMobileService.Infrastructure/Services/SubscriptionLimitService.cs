using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.Exceptions;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Services;

public class SubscriptionLimitService(AppDbContext dbContext) : ISubscriptionLimitService
{
    public async Task ValidateCanAddUserAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var usage = await GetUsageAsync(tenantId, cancellationToken);
        if (usage is null)
        {
            throw new InvalidOperationException("Tenant not found.");
        }

        if (usage.UserCount >= usage.Plan.MaxUsers)
        {
            throw new PlanLimitException("users", usage.Plan.MaxUsers);
        }
    }

    public async Task ValidateCanAddBranchAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var usage = await GetUsageAsync(tenantId, cancellationToken);
        if (usage is null)
        {
            throw new InvalidOperationException("Tenant not found.");
        }

        if (usage.BranchCount >= usage.Plan.MaxBranches)
        {
            throw new PlanLimitException("branches", usage.Plan.MaxBranches);
        }
    }

    public async Task ValidatePlanAssignmentAsync(
        Guid tenantId,
        Guid planId,
        CancellationToken cancellationToken = default)
    {
        var usage = await GetUsageAsync(tenantId, cancellationToken);
        var plan = await dbContext.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == planId && p.IsActive, cancellationToken);

        if (usage is null || plan is null)
        {
            throw new InvalidOperationException("Tenant or plan not found.");
        }

        if (usage.UserCount > plan.MaxUsers)
        {
            throw new PlanLimitException("users", plan.MaxUsers);
        }

        if (usage.BranchCount > plan.MaxBranches)
        {
            throw new PlanLimitException("branches", plan.MaxBranches);
        }
    }

    private async Task<TenantUsage?> GetUsageAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var tenant = await dbContext.Tenants
            .AsNoTracking()
            .Include(t => t.SubscriptionPlan)
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

        if (tenant is null)
        {
            return null;
        }

        var userCount = await dbContext.Users.CountAsync(u => u.TenantId == tenantId, cancellationToken);
        var branchCount = await dbContext.Branches.CountAsync(b => b.TenantId == tenantId, cancellationToken);

        return new TenantUsage(tenant.SubscriptionPlan, userCount, branchCount);
    }

    private sealed record TenantUsage(
        Domain.Entities.SubscriptionPlan Plan,
        int UserCount,
        int BranchCount);
}
