using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Subscription;

namespace OzoneMobileService.Application.Features.UpgradeRequests.Queries;

public sealed record GetTenantUpgradeRequestsQuery(Guid TenantId)
    : IQuery<IReadOnlyList<UpgradeRequestResponse>>;
