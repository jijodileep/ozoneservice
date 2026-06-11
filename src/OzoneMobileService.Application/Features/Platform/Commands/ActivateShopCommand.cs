using OzoneMobileService.Application.Common.Abstractions;

namespace OzoneMobileService.Application.Features.Platform.Commands;

public sealed record ActivateShopCommand(Guid TenantId) : ICommand<bool>;
