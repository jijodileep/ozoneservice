using OzoneMobileService.Application.Common.Abstractions;

namespace OzoneMobileService.Application.Features.Users.Commands;

public sealed record ResetUserPasswordCommand(
    Guid Id,
    Guid TenantId,
    Guid UserId,
    string NewPassword) : ICommand<bool>;
