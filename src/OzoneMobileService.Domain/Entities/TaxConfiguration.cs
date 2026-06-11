namespace OzoneMobileService.Domain.Entities;

/// <summary>Platform-wide GST split (Indian intrastate: CGST + SGST).</summary>
public class TaxConfiguration
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "GST Standard";

    public decimal CgstRate { get; set; }

    public decimal SgstRate { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime UpdatedAt { get; set; }
}
