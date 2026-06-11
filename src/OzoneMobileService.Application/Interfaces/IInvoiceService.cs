using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.Invoices;

namespace OzoneMobileService.Application.Interfaces;

public interface IInvoiceService
{
    Task<PagedResult<InvoiceResponse>> GetInvoicesPagedAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default);

    Task<byte[]?> GetInvoicePdfAsync(Guid invoiceId, CancellationToken cancellationToken = default);
}
