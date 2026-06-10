using OzoneMobileService.Application.DTOs.Auth;

namespace OzoneMobileService.Application.Interfaces;

public interface IAuthService
{
    Task<TokenResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<TokenResponse?> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);

    Task<UserProfileResponse?> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
}
