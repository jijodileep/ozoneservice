using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.Features.Users.Commands;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Features.Branches;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Features.Users.Handlers;

internal sealed class ResetUserPasswordCommandHandler(
    AppDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    BranchAccessService branchAccess)
    : IRequestHandler<ResetUserPasswordCommand, bool>
{
    public async Task<bool> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        if (!await branchAccess.IsTenantAdminAsync(request.UserId, cancellationToken))
        {
            return false;
        }

        var user = await dbContext.Users
            .FirstOrDefaultAsync(
                u => u.Id == request.Id && u.TenantId == request.TenantId,
                cancellationToken);

        if (user is null)
        {
            return false;
        }

        var roles = await userManager.GetRolesAsync(user);
        if (roles.Contains(Roles.TenantAdmin))
        {
            return false;
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, request.NewPassword);
        return result.Succeeded;
    }
}
