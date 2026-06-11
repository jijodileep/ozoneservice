using OzoneMobileService.Application.Common.Abstractions;

namespace OzoneMobileService.Application.Features.MobileMasters.Commands;

public sealed record DeactivateMobileBrandCommand(Guid Id) : ICommand<bool>;
