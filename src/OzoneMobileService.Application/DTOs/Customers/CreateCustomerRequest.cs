namespace OzoneMobileService.Application.DTOs.Customers;

public sealed record CreateCustomerRequest(
    string Name,
    string MobileNumber,
    string? Email,
    string? Address);
