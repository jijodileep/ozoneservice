using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Auth;

namespace OzoneMobileService.Application.Features.Auth.Commands;

public sealed record LoginCommand(LoginRequest Request) : ICommand<TokenResponse?>;
