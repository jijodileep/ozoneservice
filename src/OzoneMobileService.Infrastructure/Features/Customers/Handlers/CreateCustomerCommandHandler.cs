using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Customers;
using OzoneMobileService.Application.Exceptions;
using OzoneMobileService.Application.Features.Customers.Commands;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Features.Customers.Handlers;

internal sealed class CreateCustomerCommandHandler(AppDbContext dbContext)
    : IRequestHandler<CreateCustomerCommand, CustomerDetailResponse>
{
    public async Task<CustomerDetailResponse> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        if (!PhoneNormalizer.TryNormalize(request.MobileNumber, out var mobile))
        {
            throw new ArgumentException("Invalid mobile number.");
        }

        if (await dbContext.Customers.AnyAsync(
                c => c.TenantId == request.TenantId && c.MobileNumber == mobile,
                cancellationToken))
        {
            throw new CustomerDuplicateException(mobile);
        }

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            Name = request.Name.Trim(),
            MobileNumber = mobile,
            Email = CustomerMapper.NormalizeOptional(request.Email),
            Address = CustomerMapper.NormalizeOptional(request.Address),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CustomerMapper.MapDetail(customer, []);
    }
}
