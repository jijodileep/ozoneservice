using OzoneMobileService.Application.Common.Abstractions;

namespace OzoneMobileService.Application.Features.Platform.Commands;

public sealed record SuspendShopCommand(Guid TenantId) : ICommand<bool>;
