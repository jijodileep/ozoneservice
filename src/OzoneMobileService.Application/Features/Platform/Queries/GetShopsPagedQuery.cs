using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.Platform;

namespace OzoneMobileService.Application.Features.Platform.Queries;

public sealed record GetShopsPagedQuery(int Page, int PageSize, string? Search)
    : IQuery<PagedResult<ShopResponse>>;
