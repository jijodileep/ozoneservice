using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Users;

namespace OzoneMobileService.Application.Features.Users.Commands;

public sealed record CreateUserCommand(
    Guid TenantId,
    Guid UserId,
    string Email,
    string DisplayName,
    string Password,
    string Role,
    IReadOnlyList<Guid> BranchIds) : ICommand<UserResponse?>;
