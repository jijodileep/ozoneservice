namespace OzoneMobileService.Application.DTOs.Invoices;

public sealed record InvoiceResponse(
    Guid Id,
    string InvoiceNumber,
    string CustomerName,
    string CustomerPhone,
    decimal SubTotal,
    decimal CgstAmount,
    decimal SgstAmount,
    decimal TaxAmount,
    decimal TotalAmount,
    string InvoiceType,
    DateTime IssuedAt,
    string Status);
