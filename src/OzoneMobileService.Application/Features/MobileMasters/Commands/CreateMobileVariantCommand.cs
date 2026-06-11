using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.MobileMasters;

namespace OzoneMobileService.Application.Features.MobileMasters.Commands;

public sealed record CreateMobileVariantCommand(Guid ModelId, string Name)
    : ICommand<MobileVariantResponse?>;
