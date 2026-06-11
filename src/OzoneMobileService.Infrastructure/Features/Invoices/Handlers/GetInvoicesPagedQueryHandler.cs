using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.Invoices;
using OzoneMobileService.Application.Features.Invoices.Queries;
using OzoneMobileService.Infrastructure.Features.Invoices;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Invoices.Handlers;

internal sealed class GetInvoicesPagedQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetInvoicesPagedQuery, PagedResult<InvoiceResponse>>
{
    public async Task<PagedResult<InvoiceResponse>> Handle(
        GetInvoicesPagedQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = dbContext.Invoices.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(i =>
                i.InvoiceNumber.ToLower().Contains(term) ||
                i.CustomerName.ToLower().Contains(term) ||
                i.CustomerPhone.Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var invoices = await query
            .OrderByDescending(i => i.IssuedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<InvoiceResponse>(
            invoices.Select(InvoiceMapper.Map).ToList(),
            total,
            page,
            pageSize);
    }
}
