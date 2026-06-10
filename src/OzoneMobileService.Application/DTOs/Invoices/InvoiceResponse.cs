namespace OzoneMobileService.Application.DTOs.Invoices;

public sealed record InvoiceResponse(
    Guid Id,
    string InvoiceNumber,
    string CustomerName,
    string CustomerPhone,
    decimal SubTotal,
    decimal TaxAmount,
    decimal TotalAmount,
    DateTime IssuedAt,
    string Status);
