using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.MobileMasters;

namespace OzoneMobileService.Application.Features.MobileMasters.Queries;

public sealed record GetMobileBrandsPagedQuery(
    bool ActiveOnly = false,
    string? Search = null,
    int Page = 1,
    int PageSize = 20)
    : IQuery<PagedResult<MobileBrandResponse>>;
