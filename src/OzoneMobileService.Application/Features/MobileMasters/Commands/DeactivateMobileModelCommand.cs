using OzoneMobileService.Application.Common.Abstractions;

namespace OzoneMobileService.Application.Features.MobileMasters.Commands;

public sealed record DeactivateMobileModelCommand(Guid Id) : ICommand<bool>;
