using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Application.Features.Platform.Commands;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Features.Platform.Handlers;

internal sealed class CreateShopCommandHandler(
    AppDbContext dbContext,
    UserManager<ApplicationUser> userManager)
    : IRequestHandler<CreateShopCommand, ShopResponse?>
{
    public async Task<ShopResponse?> Handle(
        CreateShopCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
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

            return PlatformMapper.MapShop(tenant, 1, 1, plan);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
