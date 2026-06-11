namespace OzoneMobileService.Application.DTOs.Platform;

public sealed record TaxConfigurationResponse(
    Guid Id,
    string Name,
    decimal CgstRate,
    decimal SgstRate,
    decimal TotalGstRate,
    bool IsActive,
    DateTime UpdatedAt);

public sealed record UpdateTaxConfigurationRequest(
    string Name,
    decimal CgstRate,
    decimal SgstRate);
