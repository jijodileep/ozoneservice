namespace OzoneMobileService.Application.DTOs.Customers;

public sealed record CustomerDeviceResponse(
    Guid Id,
    Guid VariantId,
    string BrandName,
    string ModelName,
    string VariantName,
    string? Imei,
    Guid RegisteredAtBranchId,
    string RegisteredAtBranchName,
    bool IsActive,
    DateTime CreatedAt);
