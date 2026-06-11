using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.MobileMasters;

namespace OzoneMobileService.Application.Features.MobileMasters.Commands;

public sealed record UpdateMobileVariantCommand(Guid Id, string Name, bool IsActive)
    : ICommand<MobileVariantResponse?>;
