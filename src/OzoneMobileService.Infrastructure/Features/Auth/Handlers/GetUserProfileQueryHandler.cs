using MediatR;
using Microsoft.AspNetCore.Identity;
using OzoneMobileService.Application.DTOs.Auth;
using OzoneMobileService.Application.Features.Auth.Queries;
using OzoneMobileService.Domain.Entities;

namespace OzoneMobileService.Infrastructure.Features.Auth.Handlers;

internal sealed class GetUserProfileQueryHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<GetUserProfileQuery, UserProfileResponse?>
{
    public async Task<UserProfileResponse?> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return null;
        }

        var roles = await userManager.GetRolesAsync(user);

        return new UserProfileResponse(
            user.Id,
            user.Email ?? string.Empty,
            user.DisplayName,
            user.TenantId,
            roles.ToList());
    }
}
