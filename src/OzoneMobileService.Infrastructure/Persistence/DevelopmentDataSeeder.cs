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
        await SeedDevInvoiceAsync(db);
        await PatchDevTenantSubscriptionAsync(db);
    }

    private static async Task PatchDevTenantSubscriptionAsync(AppDbContext db)
    {
        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Id == SeedConstants.DevTenantId);
        if (tenant is null || tenant.SubscriptionExpiresAt.HasValue)
        {
            return;
        }

        var plan = await db.SubscriptionPlans.FirstAsync(p => p.Id == tenant.SubscriptionPlanId);
        tenant.SubscriptionExpiresAt = DateTime.UtcNow.AddMonths(plan.BillingPeriodMonths);
        await db.SaveChangesAsync();
    }

    private static async Task SeedDevInvoiceAsync(AppDbContext db)
    {
        if (await db.Invoices.AnyAsync(i => i.TenantId == SeedConstants.DevTenantId))
        {
            return;
        }

        db.Invoices.Add(new Invoice
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000020"),
            TenantId = SeedConstants.DevTenantId,
            BranchId = SeedConstants.DevBranchId,
            InvoiceNumber = "INV-DEV-001",
            CustomerName = "Sample Customer",
            CustomerPhone = "9876543210",
            SubTotal = 1500,
            TaxAmount = 270,
            TotalAmount = 1770,
            IssuedAt = DateTime.UtcNow.AddDays(-2),
            Status = "Issued",
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }

    private static async Task SeedPlansAsync(AppDbContext db)
    {
        if (!await db.SubscriptionPlans.AnyAsync())
        {
            db.SubscriptionPlans.AddRange(
                CreateStarterPlan(),
                CreateProPlan(),
                CreateEnterprisePlan());
            await db.SaveChangesAsync();
        }
        else
        {
            await PatchPlanAsync(db, SeedConstants.StarterPlanId, CreateStarterPlan());
            await PatchPlanAsync(db, SeedConstants.ProPlanId, CreateProPlan());
            await PatchPlanAsync(db, SeedConstants.EnterprisePlanId, CreateEnterprisePlan());
            await db.SaveChangesAsync();
        }
    }

    private static SubscriptionPlan CreateStarterPlan() => new()
    {
        Id = SeedConstants.StarterPlanId,
        Name = "Starter",
        Code = "STARTER",
        MaxUsers = 3,
        MaxBranches = 1,
        MaxDevicesPerUser = 1,
        Price = 999,
        BillingPeriodMonths = 1,
        TierOrder = 1
    };

    private static SubscriptionPlan CreateProPlan() => new()
    {
        Id = SeedConstants.ProPlanId,
        Name = "Pro",
        Code = "PRO",
        MaxUsers = 10,
        MaxBranches = 3,
        MaxDevicesPerUser = 1,
        Price = 2999,
        BillingPeriodMonths = 1,
        TierOrder = 2
    };

    private static SubscriptionPlan CreateEnterprisePlan() => new()
    {
        Id = SeedConstants.EnterprisePlanId,
        Name = "Enterprise",
        Code = "ENTERPRISE",
        MaxUsers = 50,
        MaxBranches = 20,
        MaxDevicesPerUser = 2,
        Price = 9999,
        BillingPeriodMonths = 12,
        TierOrder = 3
    };

    private static async Task PatchPlanAsync(AppDbContext db, Guid id, SubscriptionPlan template)
    {
        var plan = await db.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == id);
        if (plan is null)
        {
            db.SubscriptionPlans.Add(template);
            return;
        }

        plan.Price = template.Price;
        plan.BillingPeriodMonths = template.BillingPeriodMonths;
        plan.TierOrder = template.TierOrder;
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
