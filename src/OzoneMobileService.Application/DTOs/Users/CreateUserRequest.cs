namespace OzoneMobileService.Application.DTOs.Users;

public sealed record CreateUserRequest(
    string Email,
    string DisplayName,
    string Password,
    string Role,
    IReadOnlyList<Guid> BranchIds);
