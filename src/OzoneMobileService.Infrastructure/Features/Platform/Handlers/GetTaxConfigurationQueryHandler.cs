using MediatR;
using OzoneMobileService.Application.DTOs.Platform;
using OzoneMobileService.Application.Features.Platform.Queries;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Platform.Handlers;

internal sealed class GetTaxConfigurationQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetTaxConfigurationQuery, TaxConfigurationResponse>
{
    public async Task<TaxConfigurationResponse> Handle(
        GetTaxConfigurationQuery request,
        CancellationToken cancellationToken)
    {
        var config = await TaxConfigurationAccessor.GetOrCreateActiveAsync(dbContext, cancellationToken);
        return PlatformMapper.MapTaxConfiguration(config);
    }
}
