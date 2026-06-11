using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Auth;
using OzoneMobileService.Application.Features.Auth.Commands;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Features.Auth;
using OzoneMobileService.Infrastructure.Features.Notifications;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Auth.Handlers;

internal sealed class LoginCommandHandler(
    UserManager<ApplicationUser> userManager,
    AppDbContext dbContext,
    AuthTokenIssuer authTokenIssuer,
    SubscriptionExpiryChecker subscriptionExpiryChecker)
    : IRequestHandler<LoginCommand, TokenResponse?>
{
    public async Task<TokenResponse?> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Request.Password))
        {
            return null;
        }

        if (user.TenantId.HasValue)
        {
            var tenant = await dbContext.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == user.TenantId.Value, cancellationToken);

            if (tenant is null || !tenant.IsActive)
            {
                return null;
            }
        }

        var tokens = await authTokenIssuer.IssueTokensAsync(user, cancellationToken);
        await subscriptionExpiryChecker.CheckAsync(cancellationToken);
        return tokens;
    }
}
