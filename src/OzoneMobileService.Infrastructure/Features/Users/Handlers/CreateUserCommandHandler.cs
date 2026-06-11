using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OzoneMobileService.Application.DTOs.Users;
using OzoneMobileService.Application.Features.Users.Commands;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Features.Branches;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Users.Handlers;

internal sealed class CreateUserCommandHandler(
    AppDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    BranchAccessService branchAccess,
    ISubscriptionLimitService subscriptionLimitService)
    : IRequestHandler<CreateUserCommand, UserResponse?>
{
    public async Task<UserResponse?> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (!await branchAccess.IsTenantAdminAsync(request.UserId, cancellationToken))
        {
            return null;
        }

        if (!await UserMapper.ValidateBranchIdsAsync(
                dbContext,
                request.TenantId,
                request.BranchIds,
                cancellationToken))
        {
            return null;
        }

        await subscriptionLimitService.ValidateCanAddUserAsync(request.TenantId, cancellationToken);

        var email = request.Email.Trim().ToLowerInvariant();
        if (await userManager.FindByEmailAsync(email) is not null)
        {
            return null;
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            DisplayName = request.DisplayName.Trim(),
            TenantId = request.TenantId,
            IsActive = true
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            throw new ValidationException(
                createResult.Errors.Select(e => new ValidationFailure(
                    e.Code.StartsWith("Password", StringComparison.Ordinal)
                        ? nameof(CreateUserCommand.Password)
                        : nameof(CreateUserCommand.Email),
                    e.Description)));
        }

        var roleResult = await userManager.AddToRoleAsync(user, request.Role);
        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return null;
        }

        await UserMapper.SyncUserBranchesAsync(dbContext, user.Id, request.BranchIds, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var branches = await UserMapper.LoadBranchSummariesAsync(dbContext, user.Id, cancellationToken);
        return UserMapper.Map(user, request.Role, branches);
    }
}
