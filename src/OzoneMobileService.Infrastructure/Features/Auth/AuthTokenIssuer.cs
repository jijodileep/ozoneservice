using Microsoft.AspNetCore.Identity;
using OzoneMobileService.Application.DTOs.Auth;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Identity;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Auth;

internal sealed class AuthTokenIssuer(
    UserManager<ApplicationUser> userManager,
    AppDbContext dbContext,
    JwtTokenService jwtTokenService)
{
    internal async Task<TokenResponse> IssueTokensAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        var roles = await userManager.GetRolesAsync(user);
        var accessToken = jwtTokenService.GenerateAccessToken(user, roles);
        var refreshTokenValue = JwtTokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenValue,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = jwtTokenService.GetRefreshTokenExpiration()
        };

        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new TokenResponse(
            accessToken,
            refreshTokenValue,
            jwtTokenService.GetAccessTokenExpiration(),
            refreshToken.ExpiresAt);
    }
}
