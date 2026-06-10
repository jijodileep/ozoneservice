namespace OzoneMobileService.Application.DTOs.Auth;

public sealed record UserProfileResponse(
    Guid Id,
    string Email,
    string DisplayName,
    Guid? TenantId,
    IReadOnlyList<string> Roles);
