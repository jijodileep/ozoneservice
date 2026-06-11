using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.MobileMasters;

namespace OzoneMobileService.Application.Features.MobileMasters.Queries;

public sealed record GetMobileBrandsQuery(bool ActiveOnly = false)
    : IQuery<IReadOnlyList<MobileBrandResponse>>;
