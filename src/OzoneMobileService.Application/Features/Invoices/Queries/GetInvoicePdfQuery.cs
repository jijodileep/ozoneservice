using OzoneMobileService.Application.Common.Abstractions;

namespace OzoneMobileService.Application.Features.Invoices.Queries;

public sealed record GetInvoicePdfQuery(Guid InvoiceId) : IQuery<byte[]?>;
