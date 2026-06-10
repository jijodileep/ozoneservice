using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Services;

public class PlatformService(
    AppDbContext dbContext,
    UserManager<ApplicationUser> userManager) : IPlatformService
{
    public async Task<IReadOnlyList<SubscriptionPlanResponse>> GetPlansAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.SubscriptionPlans
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(p => MapPlan(p))
            .ToListAsync(cancellationToken);
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
            AllowWebLogin = request.AllowWebLogin,
            AllowMobileLogin = request.AllowMobileLogin,
            IsActive = true
        };

        dbContext.SubscriptionPlans.Add(plan);
        await dbContext.SaveChangesAsync(cancellationToken);
        return MapPlan(plan);
    }

    public async Task<IReadOnlyList<ShopResponse>> GetShopsAsync(
        CancellationToken cancellationToken = default)
    {
        var tenants = await dbContext.Tenants
            .AsNoTracking()
            .Include(t => t.SubscriptionPlan)
            .Include(t => t.Branches)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        var userCounts = await dbContext.Users
            .Where(u => u.TenantId != null)
            .GroupBy(u => u.TenantId!.Value)
            .Select(g => new { TenantId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TenantId, x => x.Count, cancellationToken);

        return tenants
            .Select(t => new ShopResponse(
                t.Id,
                t.Name,
                t.Code,
                t.IsActive,
                t.SubscriptionPlan.Name,
                t.SubscriptionPlan.Code,
                t.Branches.Count,
                userCounts.GetValueOrDefault(t.Id),
                t.CreatedAt))
            .ToList();
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

            return new ShopResponse(
                tenant.Id,
                tenant.Name,
                tenant.Code,
                tenant.IsActive,
                plan.Name,
                plan.Code,
                1,
                1,
                tenant.CreatedAt);
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
        var tenant = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
        var plan = await dbContext.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Id == planId && p.IsActive, cancellationToken);

        if (tenant is null || plan is null)
        {
            return false;
        }

        tenant.SubscriptionPlanId = plan.Id;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static SubscriptionPlanResponse MapPlan(SubscriptionPlan plan) =>
        new(
            plan.Id,
            plan.Name,
            plan.Code,
            plan.MaxUsers,
            plan.MaxBranches,
            plan.MaxDevicesPerUser,
            plan.AllowWebLogin,
            plan.AllowMobileLogin,
            plan.IsActive);
}
