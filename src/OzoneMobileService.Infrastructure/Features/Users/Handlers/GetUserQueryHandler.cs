using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Users;
using OzoneMobileService.Application.Features.Users.Queries;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Features.Branches;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Features.Users.Handlers;

internal sealed class GetUserQueryHandler(
    AppDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    BranchAccessService branchAccess)
    : IRequestHandler<GetUserQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        if (!await branchAccess.IsTenantAdminAsync(request.UserId, cancellationToken))
        {
            return null;
        }

        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.Id == request.Id && u.TenantId == request.TenantId,
                cancellationToken);

        if (user is null)
        {
            return null;
        }

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault(r => TenantAssignableRoles.IsAssignable(r))
            ?? roles.FirstOrDefault()
            ?? string.Empty;
        var branches = await UserMapper.LoadBranchSummariesAsync(dbContext, user.Id, cancellationToken);
        return UserMapper.Map(user, role, branches);
    }
}
