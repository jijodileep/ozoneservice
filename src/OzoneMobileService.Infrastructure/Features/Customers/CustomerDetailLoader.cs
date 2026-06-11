using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Customers;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Customers;

internal static class CustomerDetailLoader
{
    internal static async Task<CustomerDetailResponse?> LoadAsync(
        AppDbContext dbContext,
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var customer = await dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);

        if (customer is null)
        {
            return null;
        }

        var devices = await dbContext.CustomerDevices
            .AsNoTracking()
            .Where(d => d.CustomerId == customerId && d.IsActive)
            .Include(d => d.Variant)
                .ThenInclude(v => v.Model)
                .ThenInclude(m => m.Brand)
            .Include(d => d.RegisteredAtBranch)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);

        return CustomerMapper.MapDetail(
            customer,
            devices.Select(CustomerMapper.MapDevice).ToList());
    }
}
