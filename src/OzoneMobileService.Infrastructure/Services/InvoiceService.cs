using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Invoices;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Services;

public class InvoiceService(AppDbContext dbContext) : IInvoiceService
{
    public async Task<IReadOnlyList<InvoiceResponse>> GetInvoicesAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Invoices
            .AsNoTracking()
            .OrderByDescending(i => i.IssuedAt)
            .Select(i => new InvoiceResponse(
                i.Id,
                i.InvoiceNumber,
                i.CustomerName,
                i.CustomerPhone,
                i.SubTotal,
                i.TaxAmount,
                i.TotalAmount,
                i.IssuedAt,
                i.Status))
            .ToListAsync(cancellationToken);
    }

    public async Task<byte[]?> GetInvoicePdfAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        var invoice = await dbContext.Invoices
            .AsNoTracking()
            .Include(i => i.Branch)
            .ThenInclude(b => b.Tenant)
            .FirstOrDefaultAsync(i => i.Id == invoiceId, cancellationToken);

        if (invoice is null)
        {
            return null;
        }

        return InvoicePdfGenerator.Generate(
            invoice,
            invoice.Branch.Tenant.Name,
            invoice.Branch.Name);
    }
}
