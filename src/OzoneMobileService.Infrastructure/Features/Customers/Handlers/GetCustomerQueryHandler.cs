using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Customers;
using OzoneMobileService.Application.Features.Customers.Queries;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Customers.Handlers;

internal sealed class GetCustomerQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetCustomerQuery, CustomerDetailResponse?>
{
    public async Task<CustomerDetailResponse?> Handle(
        GetCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var exists = await dbContext.Customers
            .AsNoTracking()
            .AnyAsync(
                c => c.Id == request.CustomerId && c.TenantId == request.TenantId,
                cancellationToken);

        return exists
            ? await CustomerDetailLoader.LoadAsync(dbContext, request.CustomerId, cancellationToken)
            : null;
    }
}
