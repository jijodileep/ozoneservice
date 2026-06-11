namespace OzoneMobileService.Application.DTOs.Users;

public sealed record UpdateUserRequest(
    string DisplayName,
    string Role,
    IReadOnlyList<Guid> BranchIds,
    bool IsActive);
