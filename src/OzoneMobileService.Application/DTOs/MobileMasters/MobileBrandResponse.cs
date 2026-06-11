namespace OzoneMobileService.Application.DTOs.MobileMasters;

public sealed record MobileBrandResponse(
    Guid Id,
    string Name,
    bool IsActive);
