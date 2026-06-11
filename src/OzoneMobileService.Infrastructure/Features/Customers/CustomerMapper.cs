using OzoneMobileService.Application.DTOs.Customers;
using OzoneMobileService.Domain.Entities;

namespace OzoneMobileService.Infrastructure.Features.Customers;

internal static class CustomerMapper
{
    internal static CustomerDetailResponse MapDetail(
        Customer customer,
        IReadOnlyList<CustomerDeviceResponse> devices) =>
        new(
            customer.Id,
            customer.Name,
            customer.MobileNumber,
            customer.Email,
            customer.Address,
            devices,
            []);

    internal static CustomerDeviceResponse MapDevice(CustomerDevice device) =>
        new(
            device.Id,
            device.VariantId,
            device.Variant.Model.Brand.Name,
            device.Variant.Model.Name,
            device.Variant.Name,
            device.Imei,
            device.RegisteredAtBranchId,
            device.RegisteredAtBranch.Name,
            device.IsActive,
            device.CreatedAt);

    internal static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
