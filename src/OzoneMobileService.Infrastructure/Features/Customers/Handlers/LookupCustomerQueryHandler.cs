using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Customers;
using OzoneMobileService.Application.Features.Customers.Queries;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Features.Customers.Handlers;

internal sealed class LookupCustomerQueryHandler(AppDbContext dbContext)
    : IRequestHandler<LookupCustomerQuery, CustomerDetailResponse?>
{
    public async Task<CustomerDetailResponse?> Handle(
        LookupCustomerQuery request,
        CancellationToken cancellationToken)
    {
        if (!PhoneNormalizer.TryNormalize(request.Mobile, out var mobile))
        {
            return null;
        }

        var customerId = await dbContext.Customers
            .AsNoTracking()
            .Where(c => c.TenantId == request.TenantId && c.MobileNumber == mobile)
            .Select(c => c.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (customerId == Guid.Empty)
        {
            return null;
        }

        return await CustomerDetailLoader.LoadAsync(dbContext, customerId, cancellationToken);
    }
}
