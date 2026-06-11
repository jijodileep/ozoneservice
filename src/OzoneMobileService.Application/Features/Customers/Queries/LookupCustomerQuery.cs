using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Customers;

namespace OzoneMobileService.Application.Features.Customers.Queries;

public sealed record LookupCustomerQuery(Guid TenantId, string Mobile)
    : IQuery<CustomerDetailResponse?>;
