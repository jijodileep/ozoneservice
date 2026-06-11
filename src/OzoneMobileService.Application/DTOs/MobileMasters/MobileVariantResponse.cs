namespace OzoneMobileService.Application.DTOs.MobileMasters;

public sealed record MobileVariantResponse(
    Guid Id,
    Guid ModelId,
    string Name,
    bool IsActive);
