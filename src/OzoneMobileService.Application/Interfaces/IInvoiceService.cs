using OzoneMobileService.Application.DTOs.Invoices;

namespace OzoneMobileService.Application.Interfaces;

public interface IInvoiceService
{
    Task<IReadOnlyList<InvoiceResponse>> GetInvoicesAsync(CancellationToken cancellationToken = default);

    Task<byte[]?> GetInvoicePdfAsync(Guid invoiceId, CancellationToken cancellationToken = default);
}
