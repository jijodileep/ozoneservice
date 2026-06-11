using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Customers;
using OzoneMobileService.Application.Features.Customers.Commands;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Customers.Handlers;

internal sealed class AddCustomerDeviceCommandHandler(AppDbContext dbContext)
    : IRequestHandler<AddCustomerDeviceCommand, CustomerDeviceResponse?>
{
    public async Task<CustomerDeviceResponse?> Handle(
        AddCustomerDeviceCommand request,
        CancellationToken cancellationToken)
    {
        var customerExists = await dbContext.Customers
            .AsNoTracking()
            .AnyAsync(
                c => c.Id == request.CustomerId && c.TenantId == request.TenantId,
                cancellationToken);

        if (!customerExists)
        {
            return null;
        }

        var variant = await dbContext.MobileVariants
            .AsNoTracking()
            .Include(v => v.Model)
                .ThenInclude(m => m.Brand)
            .FirstOrDefaultAsync(
                v => v.Id == request.VariantId
                    && v.IsActive
                    && v.Model.IsActive
                    && v.Model.Brand.IsActive,
                cancellationToken);

        if (variant is null)
        {
            return null;
        }

        var branchExists = await dbContext.Branches
            .AsNoTracking()
            .AnyAsync(
                b => b.Id == request.BranchId && b.TenantId == request.TenantId && b.IsActive,
                cancellationToken);

        if (!branchExists)
        {
            return null;
        }

        var device = new CustomerDevice
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            CustomerId = request.CustomerId,
            VariantId = request.VariantId,
            Imei = CustomerMapper.NormalizeOptional(request.Imei),
            RegisteredAtBranchId = request.BranchId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.CustomerDevices.Add(device);
        await dbContext.SaveChangesAsync(cancellationToken);

        device.Variant = variant;
        device.RegisteredAtBranch = await dbContext.Branches
            .AsNoTracking()
            .FirstAsync(b => b.Id == request.BranchId, cancellationToken);

        return CustomerMapper.MapDevice(device);
    }
}
