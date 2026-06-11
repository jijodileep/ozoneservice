using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.Invoices;
using OzoneMobileService.Application.Features.Invoices.Queries;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Api.Controllers;

[Route("api/invoices")]
public class InvoicesController(IMediator mediator) : ReportsApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<InvoiceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInvoices(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var invoices = await mediator.Send(new GetInvoicesPagedQuery(page, pageSize, search), cancellationToken);
        return Ok(invoices);
    }

    [Authorize(Policy = AuthorizationPolicies.SetupWrite)]
    [HttpGet("{id:guid}/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPdf(Guid id, CancellationToken cancellationToken)
    {
        var pdf = await mediator.Send(new GetInvoicePdfQuery(id), cancellationToken);
        return pdf is null ? NotFound() : File(pdf, "application/pdf", $"invoice-{id}.pdf");
    }
}
