using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Users;
using OzoneMobileService.Application.Features.Users.Commands;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Features.Branches;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Features.Users.Handlers;

internal sealed class UpdateUserCommandHandler(
    AppDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    BranchAccessService branchAccess)
    : IRequestHandler<UpdateUserCommand, UserResponse?>
{
    public async Task<UserResponse?> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        if (!await branchAccess.IsTenantAdminAsync(request.UserId, cancellationToken))
        {
            return null;
        }

        var user = await dbContext.Users
            .FirstOrDefaultAsync(
                u => u.Id == request.Id && u.TenantId == request.TenantId,
                cancellationToken);

        if (user is null)
        {
            return null;
        }

        var roles = await userManager.GetRolesAsync(user);
        if (roles.Contains(Roles.TenantAdmin))
        {
            return null;
        }

        if (!request.IsActive && user.Id == request.UserId)
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

        user.DisplayName = request.DisplayName.Trim();
        user.IsActive = request.IsActive;

        var currentRole = roles.FirstOrDefault(r => TenantAssignableRoles.IsAssignable(r));
        if (currentRole is not null && currentRole != request.Role)
        {
            await userManager.RemoveFromRoleAsync(user, currentRole);
        }

        if (!roles.Contains(request.Role))
        {
            await userManager.AddToRoleAsync(user, request.Role);
        }

        await UserMapper.SyncUserBranchesAsync(dbContext, user.Id, request.BranchIds, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var branches = await UserMapper.LoadBranchSummariesAsync(dbContext, user.Id, cancellationToken);
        return UserMapper.Map(user, request.Role, branches);
    }
}
