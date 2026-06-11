using OzoneMobileService.Application.DTOs.Platform;

namespace OzoneMobileService.Application.Interfaces;

public interface ITaxService
{
    Task<TaxConfigurationResponse> GetConfigurationAsync(CancellationToken cancellationToken = default);

    Task<TaxConfigurationResponse?> UpdateConfigurationAsync(
        UpdateTaxConfigurationRequest request,
        CancellationToken cancellationToken = default);
}
