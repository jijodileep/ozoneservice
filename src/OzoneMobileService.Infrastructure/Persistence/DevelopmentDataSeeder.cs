using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Persistence;

public static class DevelopmentDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        await SeedPlansAsync(db);
        await SeedRolesAsync(roleManager);
        await SeedDevTenantAsync(db);
        await SeedDevBranchAsync(db);
        await SeedSuperAdminAsync(userManager);
        await SeedDevTenantAdminAsync(db, userManager);
        await SeedDevShopUsersAsync(db, userManager);
    }

    private static async Task SeedPlansAsync(AppDbContext db)
    {
        if (await db.SubscriptionPlans.AnyAsync())
        {
            return;
        }

        db.SubscriptionPlans.AddRange(
            new SubscriptionPlan
            {
                Id = SeedConstants.StarterPlanId,
                Name = "Starter",
                Code = "STARTER",
                MaxUsers = 3,
                MaxBranches = 1,
                MaxDevicesPerUser = 1
            },
            new SubscriptionPlan
            {
                Id = SeedConstants.ProPlanId,
                Name = "Pro",
                Code = "PRO",
                MaxUsers = 10,
                MaxBranches = 3,
                MaxDevicesPerUser = 1
            },
            new SubscriptionPlan
            {
                Id = SeedConstants.EnterprisePlanId,
                Name = "Enterprise",
                Code = "ENTERPRISE",
                MaxUsers = 50,
                MaxBranches = 20,
                MaxDevicesPerUser = 2
            });

        await db.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        string[] roles =
        [
            Roles.PlatformSuperAdmin,
            Roles.TenantAdmin,
            Roles.ShopAdmin,
            Roles.ShopStaff,
            Roles.Accountant
        ];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = role,
                    NormalizedName = role.ToUpperInvariant()
                });
            }
        }
    }

    private static async Task SeedDevTenantAsync(AppDbContext db)
    {
        if (await db.Tenants.AnyAsync(t => t.Id == SeedConstants.DevTenantId))
        {
            return;
        }

        db.Tenants.Add(new Tenant
        {
            Id = SeedConstants.DevTenantId,
            Name = "Dev Service Center",
            Code = "DEV",
            IsActive = true,
            SubscriptionPlanId = SeedConstants.StarterPlanId,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }

    private static async Task SeedDevBranchAsync(AppDbContext db)
    {
        if (await db.Branches.AnyAsync(b => b.TenantId == SeedConstants.DevTenantId))
        {
            return;
        }

        db.Branches.Add(new Branch
        {
            Id = SeedConstants.DevBranchId,
            TenantId = SeedConstants.DevTenantId,
            Code = "MAIN",
            Name = "Main Branch",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }

    private static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager)
    {
        if (await userManager.FindByEmailAsync(SeedConstants.SuperAdminEmail) is not null)
        {
            return;
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = SeedConstants.SuperAdminEmail,
            Email = SeedConstants.SuperAdminEmail,
            EmailConfirmed = true,
            DisplayName = SeedConstants.SuperAdminDisplayName,
            TenantId = null
        };

        var result = await userManager.CreateAsync(user, SeedConstants.SuperAdminPassword);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to seed super admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        await userManager.AddToRoleAsync(user, Roles.PlatformSuperAdmin);
    }

    private static async Task SeedDevTenantAdminAsync(
        AppDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        if (await userManager.FindByEmailAsync(SeedConstants.DevAdminEmail) is not null)
        {
            return;
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = SeedConstants.DevAdminEmail,
            Email = SeedConstants.DevAdminEmail,
            EmailConfirmed = true,
            DisplayName = SeedConstants.DevAdminDisplayName,
            TenantId = SeedConstants.DevTenantId
        };

        var result = await userManager.CreateAsync(user, SeedConstants.DevAdminPassword);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to seed dev user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        await userManager.AddToRoleAsync(user, Roles.TenantAdmin);
    }

    private static async Task SeedDevShopUsersAsync(
        AppDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        await SeedDevUserWithBranchAsync(
            db, userManager,
            SeedConstants.DevShopAdminEmail,
            SeedConstants.DevShopAdminPassword,
            SeedConstants.DevShopAdminDisplayName,
            Roles.ShopAdmin);

        await SeedDevUserWithBranchAsync(
            db, userManager,
            SeedConstants.DevStaffEmail,
            SeedConstants.DevStaffPassword,
            SeedConstants.DevStaffDisplayName,
            Roles.ShopStaff);

        await SeedDevUserWithBranchAsync(
            db, userManager,
            SeedConstants.DevAccountantEmail,
            SeedConstants.DevAccountantPassword,
            SeedConstants.DevAccountantDisplayName,
            Roles.Accountant);
    }

    private static async Task SeedDevUserWithBranchAsync(
        AppDbContext db,
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string displayName,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                DisplayName = displayName,
                TenantId = SeedConstants.DevTenantId
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to seed {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await userManager.AddToRoleAsync(user, role);
        }

        var hasBranch = await db.UserBranches
            .AnyAsync(ub => ub.UserId == user.Id && ub.BranchId == SeedConstants.DevBranchId);

        if (!hasBranch)
        {
            db.UserBranches.Add(new UserBranch
            {
                UserId = user.Id,
                BranchId = SeedConstants.DevBranchId
            });

            await db.SaveChangesAsync();
        }
    }

    public static async Task SeedDevelopmentDataAsync(this IHost app)
    {
        var environment = app.Services.GetRequiredService<IHostEnvironment>();
        if (!environment.IsDevelopment())
        {
            return;
        }

        await SeedAsync(app.Services);
    }
}
