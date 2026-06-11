namespace OzoneMobileService.Application.DTOs.Customers;

public sealed record CustomerDetailResponse(
    Guid Id,
    string Name,
    string MobileNumber,
    string? Email,
    string? Address,
    IReadOnlyList<CustomerDeviceResponse> Devices,
    IReadOnlyList<CustomerHistoryItem> History);
