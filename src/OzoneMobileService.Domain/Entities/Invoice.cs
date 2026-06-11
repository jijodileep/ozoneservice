using OzoneMobileService.Domain.Common;

namespace OzoneMobileService.Domain.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;

    public Guid BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public string CustomerName { get; set; } = string.Empty;

    public string CustomerPhone { get; set; } = string.Empty;

    public decimal SubTotal { get; set; }

    public decimal CgstAmount { get; set; }

    public decimal SgstAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string InvoiceType { get; set; } = "Service";

    public DateTime IssuedAt { get; set; }

    public string Status { get; set; } = "Issued";
}
