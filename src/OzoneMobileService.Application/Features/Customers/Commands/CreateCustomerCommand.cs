using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Customers;

namespace OzoneMobileService.Application.Features.Customers.Commands;

public sealed record CreateCustomerCommand(
    Guid TenantId,
    string Name,
    string MobileNumber,
    string? Email,
    string? Address) : ICommand<CustomerDetailResponse>;
