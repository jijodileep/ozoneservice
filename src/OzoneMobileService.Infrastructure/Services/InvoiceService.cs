using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.Invoices;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Services;

public class InvoiceService(AppDbContext dbContext) : IInvoiceService
{
    public async Task<PagedResult<InvoiceResponse>> GetInvoicesPagedAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = dbContext.Invoices.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(i =>
                i.InvoiceNumber.ToLower().Contains(term) ||
                i.CustomerName.ToLower().Contains(term) ||
                i.CustomerPhone.Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(i => i.IssuedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(i => Map(i))
            .ToListAsync(cancellationToken);

        return new PagedResult<InvoiceResponse>(items, total, page, pageSize);
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

    private static InvoiceResponse Map(Domain.Entities.Invoice i) =>
        new(
            i.Id,
            i.InvoiceNumber,
            i.CustomerName,
            i.CustomerPhone,
            i.SubTotal,
            i.CgstAmount,
            i.SgstAmount,
            i.TaxAmount,
            i.TotalAmount,
            i.InvoiceType,
            i.IssuedAt,
            i.Status);
}
