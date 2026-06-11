using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.MobileMasters;

namespace OzoneMobileService.Application.Features.MobileMasters.Commands;

public sealed record CreateMobileModelCommand(Guid TenantId, Guid BrandId, string Name)
    : ICommand<MobileModelResponse?>;
