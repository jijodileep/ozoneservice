using MediatR;
using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Application.Features.Platform.Commands;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Platform.Handlers;

internal sealed class UpdateTaxConfigurationCommandHandler(AppDbContext dbContext)
    : IRequestHandler<UpdateTaxConfigurationCommand, TaxConfigurationResponse?>
{
    public async Task<TaxConfigurationResponse?> Handle(
        UpdateTaxConfigurationCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        if (request.CgstRate < 0 || request.SgstRate < 0)
        {
            return null;
        }

        var config = await TaxConfigurationAccessor.GetOrCreateActiveAsync(dbContext, cancellationToken);
        config.Name = request.Name.Trim();
        config.CgstRate = request.CgstRate;
        config.SgstRate = request.SgstRate;
        config.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return PlatformMapper.MapTaxConfiguration(config);
    }
}
