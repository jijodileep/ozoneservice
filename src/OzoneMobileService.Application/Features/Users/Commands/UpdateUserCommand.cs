using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Users;

namespace OzoneMobileService.Application.Features.Users.Commands;

public sealed record UpdateUserCommand(
    Guid Id,
    Guid TenantId,
    Guid UserId,
    string DisplayName,
    string Role,
    IReadOnlyList<Guid> BranchIds,
    bool IsActive) : ICommand<UserResponse?>;
