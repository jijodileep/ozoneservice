using OzoneMobileService.Application.Common.Abstractions;

namespace OzoneMobileService.Application.Features.Auth.Commands;

public sealed record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword) : ICommand<bool>;
