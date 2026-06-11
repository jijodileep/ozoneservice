using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Application.Exceptions;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Services;

public class PlatformService(
    AppDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    ISubscriptionLimitService subscriptionLimitService) : IPlatformService
{
    public async Task<IReadOnlyList<SubscriptionPlanResponse>> GetPlansAsync(
        CancellationToken cancellationToken = default)
    {
        var plans = await dbContext.SubscriptionPlans
            .AsNoTracking()
            .OrderBy(p => p.TierOrder)
            .ToListAsync(cancellationToken);

        var tenantCounts = await dbContext.Tenants
            .GroupBy(t => t.SubscriptionPlanId)
            .Select(g => new { PlanId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PlanId, x => x.Count, cancellationToken);

        return plans
            .Select(p => MapPlan(p, tenantCounts.GetValueOrDefault(p.Id)))
            .ToList();
    }

    public async Task<SubscriptionPlanResponse?> CreatePlanAsync(
        CreateSubscriptionPlanRequest request,
        CancellationToken cancellationToken = default)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (await dbContext.SubscriptionPlans.AnyAsync(p => p.Code == code, cancellationToken))
        {
            return null;
        }

        var plan = new SubscriptionPlan
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Code = code,
            MaxUsers = request.MaxUsers,
            MaxBranches = request.MaxBranches,
            MaxDevicesPerUser = request.MaxDevicesPerUser,
            Price = request.Price,
            BillingPeriodMonths = request.BillingPeriodMonths,
            TierOrder = request.TierOrder,
            AllowWebLogin = request.AllowWebLogin,
            AllowMobileLogin = request.AllowMobileLogin,
            IsActive = true
        };

        dbContext.SubscriptionPlans.Add(plan);
        await dbContext.SaveChangesAsync(cancellationToken);
        return MapPlan(plan, 0);
    }

    public async Task<SubscriptionPlanResponse?> UpdatePlanAsync(
        Guid planId,
        UpdateSubscriptionPlanRequest request,
        CancellationToken cancellationToken = default)
    {
        var plan = await dbContext.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == planId, cancellationToken);
        if (plan is null)
        {
            return null;
        }

        plan.Name = request.Name.Trim();
        plan.MaxUsers = request.MaxUsers;
        plan.MaxBranches = request.MaxBranches;
        plan.MaxDevicesPerUser = request.MaxDevicesPerUser;
        plan.Price = request.Price;
        plan.BillingPeriodMonths = request.BillingPeriodMonths;
        plan.TierOrder = request.TierOrder;
        plan.AllowWebLogin = request.AllowWebLogin;
        plan.AllowMobileLogin = request.AllowMobileLogin;
        plan.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);

        var tenantCount = await dbContext.Tenants.CountAsync(t => t.SubscriptionPlanId == planId, cancellationToken);
        return MapPlan(plan, tenantCount);
    }

    public async Task<bool> DeletePlanAsync(Guid planId, CancellationToken cancellationToken = default)
    {
        var plan = await dbContext.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == planId, cancellationToken);
        if (plan is null)
        {
            return false;
        }

        if (await dbContext.Tenants.AnyAsync(t => t.SubscriptionPlanId == planId, cancellationToken))
        {
            throw new PlanInUseException();
        }

        dbContext.SubscriptionPlans.Remove(plan);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<ShopResponse>> GetShopsAsync(
        CancellationToken cancellationToken = default)
    {
        var paged = await GetShopsPagedAsync(1, int.MaxValue, null, cancellationToken);
        return paged.Items;
    }

    public async Task<PagedResult<ShopResponse>> GetShopsPagedAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = dbContext.Tenants
            .AsNoTracking()
            .Include(t => t.SubscriptionPlan)
            .Include(t => t.Branches)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(t =>
                t.Name.ToLower().Contains(term) ||
                t.Code.ToLower().Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var tenants = await query
            .OrderBy(t => t.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var tenantIds = tenants.Select(t => t.Id).ToList();
        var userCounts = await dbContext.Users
            .Where(u => u.TenantId != null && tenantIds.Contains(u.TenantId.Value))
            .GroupBy(u => u.TenantId!.Value)
            .Select(g => new { TenantId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TenantId, x => x.Count, cancellationToken);

        var items = tenants
            .Select(t => MapShop(t, userCounts.GetValueOrDefault(t.Id), t.Branches.Count))
            .ToList();

        return new PagedResult<ShopResponse>(items, total, page, pageSize);
    }

    public async Task<ShopResponse?> CreateShopAsync(
        CreateShopRequest request,
        CancellationToken cancellationToken = default)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        var plan = await dbContext.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Id == request.SubscriptionPlanId && p.IsActive, cancellationToken);

        if (plan is null)
        {
            return null;
        }

        if (await dbContext.Tenants.AnyAsync(t => t.Code == code, cancellationToken))
        {
            return null;
        }

        if (await userManager.FindByEmailAsync(request.ShopAdminEmail) is not null)
        {
            return null;
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var tenantId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var tenant = new Tenant
            {
                Id = tenantId,
                Name = request.Name.Trim(),
                Code = code,
                IsActive = true,
                SubscriptionPlanId = plan.Id,
                SubscriptionExpiresAt = now.AddMonths(plan.BillingPeriodMonths),
                CreatedAt = now
            };

            var branch = new Branch
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Code = "MAIN",
                Name = request.DefaultBranchName.Trim(),
                IsActive = true,
                CreatedAt = now
            };

            var shopAdmin = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = request.ShopAdminEmail.Trim(),
                Email = request.ShopAdminEmail.Trim(),
                EmailConfirmed = true,
                DisplayName = request.ShopAdminDisplayName.Trim(),
                TenantId = tenantId
            };

            dbContext.Tenants.Add(tenant);
            dbContext.Branches.Add(branch);
            await dbContext.SaveChangesAsync(cancellationToken);

            var createResult = await userManager.CreateAsync(shopAdmin, request.ShopAdminPassword);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }

            await userManager.AddToRoleAsync(shopAdmin, Roles.ShopAdmin);

            dbContext.UserBranches.Add(new UserBranch
            {
                UserId = shopAdmin.Id,
                BranchId = branch.Id
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return MapShop(tenant, 1, 1, plan);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> SuspendShopAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
        if (tenant is null)
        {
            return false;
        }

        tenant.IsActive = false;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ActivateShopAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
        if (tenant is null)
        {
            return false;
        }

        tenant.IsActive = true;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> AssignPlanAsync(
        Guid tenantId,
        Guid planId,
        CancellationToken cancellationToken = default)
    {
        var tenant = await dbContext.Tenants
            .Include(t => t.SubscriptionPlan)
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
        var plan = await dbContext.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Id == planId && p.IsActive, cancellationToken);

        if (tenant is null || plan is null)
        {
            return false;
        }

        await subscriptionLimitService.ValidatePlanAssignmentAsync(tenantId, planId, cancellationToken);

        tenant.SubscriptionPlanId = plan.Id;
        tenant.SubscriptionExpiresAt = DateTime.UtcNow.AddMonths(plan.BillingPeriodMonths);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static SubscriptionPlanResponse MapPlan(SubscriptionPlan plan, int tenantCount) =>
        new(
            plan.Id,
            plan.Name,
            plan.Code,
            plan.MaxUsers,
            plan.MaxBranches,
            plan.MaxDevicesPerUser,
            plan.Price,
            plan.BillingPeriodMonths,
            plan.TierOrder,
            plan.AllowWebLogin,
            plan.AllowMobileLogin,
            plan.IsActive,
            tenantCount);

    private static ShopResponse MapShop(
        Tenant tenant,
        int userCount,
        int branchCount,
        SubscriptionPlan? planOverride = null)
    {
        var plan = planOverride ?? tenant.SubscriptionPlan;
        return new ShopResponse(
            tenant.Id,
            tenant.Name,
            tenant.Code,
            tenant.IsActive,
            plan.Name,
            plan.Code,
            plan.MaxUsers,
            plan.MaxBranches,
            plan.MaxDevicesPerUser,
            branchCount,
            userCount,
            tenant.SubscriptionExpiresAt,
            tenant.CreatedAt);
    }
}
