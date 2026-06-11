using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Customers;

namespace OzoneMobileService.Application.Features.Customers.Queries;

public sealed record GetCustomerQuery(Guid TenantId, Guid CustomerId)
    : IQuery<CustomerDetailResponse?>;
