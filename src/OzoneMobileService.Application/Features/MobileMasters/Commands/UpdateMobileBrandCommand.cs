using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.MobileMasters;

namespace OzoneMobileService.Application.Features.MobileMasters.Commands;

public sealed record UpdateMobileBrandCommand(Guid Id, string Name, bool IsActive)
    : ICommand<MobileBrandResponse?>;
