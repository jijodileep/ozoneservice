using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Invoices;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Api.Controllers;

[Authorize(Policy = AuthorizationPolicies.ReportsRead)]
[ApiController]
[Route("api/invoices")]
public class InvoicesController(IInvoiceService invoiceService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<InvoiceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInvoices(CancellationToken cancellationToken)
    {
        var invoices = await invoiceService.GetInvoicesAsync(cancellationToken);
        return Ok(invoices);
    }

    [Authorize(Policy = AuthorizationPolicies.SetupWrite)]
    [HttpGet("{id:guid}/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPdf(Guid id, CancellationToken cancellationToken)
    {
        var pdf = await invoiceService.GetInvoicePdfAsync(id, cancellationToken);
        if (pdf is null)
        {
            return NotFound();
        }

        return File(pdf, "application/pdf", $"invoice-{id}.pdf");
    }
}
