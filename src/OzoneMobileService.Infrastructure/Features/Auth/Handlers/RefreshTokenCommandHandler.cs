using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Auth;
using OzoneMobileService.Application.Features.Auth.Commands;
using OzoneMobileService.Infrastructure.Features.Auth;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Auth.Handlers;

internal sealed class RefreshTokenCommandHandler(
    AppDbContext dbContext,
    AuthTokenIssuer authTokenIssuer)
    : IRequestHandler<RefreshTokenCommand, TokenResponse?>
{
    public async Task<TokenResponse?> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var storedToken = await dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.Request.RefreshToken, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
        {
            return null;
        }

        storedToken.RevokedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return await authTokenIssuer.IssueTokensAsync(storedToken.User, cancellationToken);
    }
}
