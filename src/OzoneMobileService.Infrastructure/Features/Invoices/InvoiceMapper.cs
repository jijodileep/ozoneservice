using OzoneMobileService.Application.DTOs.Invoices;
using OzoneMobileService.Domain.Entities;

namespace OzoneMobileService.Infrastructure.Features.Invoices;

internal static class InvoiceMapper
{
    internal static InvoiceResponse Map(Invoice invoice) =>
        new(
            invoice.Id,
            invoice.InvoiceNumber,
            invoice.CustomerName,
            invoice.CustomerPhone,
            invoice.SubTotal,
            invoice.CgstAmount,
            invoice.SgstAmount,
            invoice.TaxAmount,
            invoice.TotalAmount,
            invoice.InvoiceType,
            invoice.IssuedAt,
            invoice.Status);
}
