using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.Subscription;

namespace OzoneMobileService.Application.Features.UpgradeRequests.Queries;

public sealed record GetUpgradeRequestsPagedQuery(int Page, int PageSize, string? Status)
    : IQuery<PagedResult<UpgradeRequestResponse>>;
