using MediatR;
using Microsoft.AspNetCore.Identity;
using OzoneMobileService.Application.Features.Auth.Commands;
using OzoneMobileService.Domain.Entities;

namespace OzoneMobileService.Infrastructure.Features.Auth.Handlers;

internal sealed class ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<ChangePasswordCommand, bool>
{
    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return false;
        }

        var result = await userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        return result.Succeeded;
    }
}
