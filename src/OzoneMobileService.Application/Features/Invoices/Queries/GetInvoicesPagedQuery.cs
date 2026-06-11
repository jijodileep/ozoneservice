using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.Invoices;

namespace OzoneMobileService.Application.Features.Invoices.Queries;

public sealed record GetInvoicesPagedQuery(int Page, int PageSize, string? Search)
    : IQuery<PagedResult<InvoiceResponse>>;
