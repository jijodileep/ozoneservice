using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Customers;

namespace OzoneMobileService.Application.Features.Customers.Commands;

public sealed record AddCustomerDeviceCommand(
    Guid TenantId,
    Guid CustomerId,
    Guid BranchId,
    Guid VariantId,
    string? Imei) : ICommand<CustomerDeviceResponse?>;
