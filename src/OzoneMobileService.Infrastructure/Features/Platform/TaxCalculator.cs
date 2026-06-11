using OzoneMobileService.Domain.Entities;

namespace OzoneMobileService.Infrastructure.Features.Platform;

internal static class TaxCalculator
{
    internal static (decimal Cgst, decimal Sgst, decimal TotalTax) CalculateTax(
        decimal subTotal,
        TaxConfiguration config)
    {
        var cgst = Math.Round(subTotal * config.CgstRate / 100m, 2);
        var sgst = Math.Round(subTotal * config.SgstRate / 100m, 2);
        return (cgst, sgst, cgst + sgst);
    }
}
