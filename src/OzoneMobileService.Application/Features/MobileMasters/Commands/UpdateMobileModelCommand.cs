using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.MobileMasters;

namespace OzoneMobileService.Application.Features.MobileMasters.Commands;

public sealed record UpdateMobileModelCommand(Guid Id, string Name, bool IsActive)
    : ICommand<MobileModelResponse?>;
