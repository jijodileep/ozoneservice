using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Platform;

namespace OzoneMobileService.Application.Features.Platform.Queries;

public sealed record GetTaxConfigurationQuery : IQuery<TaxConfigurationResponse>;
