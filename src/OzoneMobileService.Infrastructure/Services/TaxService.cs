using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Services;

public class TaxService(AppDbContext dbContext) : ITaxService
{
    public async Task<TaxConfigurationResponse> GetConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var config = await GetOrCreateActiveAsync(cancellationToken);
        return Map(config);
    }

    public async Task<TaxConfigurationResponse?> UpdateConfigurationAsync(
        UpdateTaxConfigurationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CgstRate < 0 || request.SgstRate < 0)
        {
            return null;
        }

        var config = await GetOrCreateActiveAsync(cancellationToken);
        config.Name = request.Name.Trim();
        config.CgstRate = request.CgstRate;
        config.SgstRate = request.SgstRate;
        config.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(config);
    }

    public static (decimal Cgst, decimal Sgst, decimal TotalTax) CalculateTax(
        decimal subTotal,
        TaxConfiguration config)
    {
        var cgst = Math.Round(subTotal * config.CgstRate / 100m, 2);
        var sgst = Math.Round(subTotal * config.SgstRate / 100m, 2);
        return (cgst, sgst, cgst + sgst);
    }

    private async Task<TaxConfiguration> GetOrCreateActiveAsync(CancellationToken cancellationToken)
    {
        var config = await dbContext.TaxConfigurations
            .FirstOrDefaultAsync(t => t.IsActive, cancellationToken);

        if (config is not null)
        {
            return config;
        }

        config = new TaxConfiguration
        {
            Id = Guid.NewGuid(),
            Name = "GST 18%",
            CgstRate = 9m,
            SgstRate = 9m,
            IsActive = true,
            UpdatedAt = DateTime.UtcNow
        };
        dbContext.TaxConfigurations.Add(config);
        await dbContext.SaveChangesAsync(cancellationToken);
        return config;
    }

    private static TaxConfigurationResponse Map(TaxConfiguration config) =>
        new(
            config.Id,
            config.Name,
            config.CgstRate,
            config.SgstRate,
            config.CgstRate + config.SgstRate,
            config.IsActive,
            config.UpdatedAt);
}
