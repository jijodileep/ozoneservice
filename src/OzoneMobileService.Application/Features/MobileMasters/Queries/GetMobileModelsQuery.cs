using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.MobileMasters;

namespace OzoneMobileService.Application.Features.MobileMasters.Queries;

public sealed record GetMobileModelsQuery(Guid BrandId, bool ActiveOnly = false)
    : IQuery<IReadOnlyList<MobileModelResponse>>;
