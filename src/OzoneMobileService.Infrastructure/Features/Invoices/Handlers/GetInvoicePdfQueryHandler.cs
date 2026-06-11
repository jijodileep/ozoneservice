using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.Features.Invoices.Queries;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Infrastructure.Services;

namespace OzoneMobileService.Infrastructure.Features.Invoices.Handlers;

internal sealed class GetInvoicePdfQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetInvoicePdfQuery, byte[]?>
{
    public async Task<byte[]?> Handle(
        GetInvoicePdfQuery request,
        CancellationToken cancellationToken)
    {
        var invoice = await dbContext.Invoices
            .AsNoTracking()
            .Include(i => i.Branch)
            .ThenInclude(b => b.Tenant)
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

        if (invoice is null)
        {
            return null;
        }

        var taxConfig = await dbContext.TaxConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.IsActive, cancellationToken);

        return InvoicePdfGenerator.Generate(
            invoice,
            invoice.Branch.Tenant.Name,
            invoice.Branch.Name,
            taxConfig?.CgstRate ?? 0,
            taxConfig?.SgstRate ?? 0);
    }
}
