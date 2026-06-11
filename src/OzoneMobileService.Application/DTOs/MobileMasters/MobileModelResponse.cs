namespace OzoneMobileService.Application.DTOs.MobileMasters;

public sealed record MobileModelResponse(
    Guid Id,
    Guid BrandId,
    string Name,
    bool IsActive,
    Guid TenantId);
