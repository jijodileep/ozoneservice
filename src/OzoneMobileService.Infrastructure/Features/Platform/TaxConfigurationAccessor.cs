using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Platform;

internal static class TaxConfigurationAccessor
{
    internal static async Task<TaxConfiguration> GetOrCreateActiveAsync(
        AppDbContext dbContext,
        CancellationToken cancellationToken)
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
}
