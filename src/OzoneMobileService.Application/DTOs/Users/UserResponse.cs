namespace OzoneMobileService.Application.DTOs.Users;

public sealed record UserResponse(
    Guid Id,
    string Email,
    string DisplayName,
    string Role,
    bool IsActive,
    IReadOnlyList<UserBranchSummary> Branches);
