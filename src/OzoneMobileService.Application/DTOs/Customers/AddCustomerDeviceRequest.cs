namespace OzoneMobileService.Application.DTOs.Customers;

public sealed record AddCustomerDeviceRequest(
    Guid VariantId,
    string? Imei);
