using OzoneMobileService.Application.Common.Abstractions;

namespace OzoneMobileService.Application.Features.MobileMasters.Commands;

public sealed record DeactivateMobileVariantCommand(Guid Id) : ICommand<bool>;
