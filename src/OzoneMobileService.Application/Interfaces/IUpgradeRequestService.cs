using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.Subscription;

namespace OzoneMobileService.Application.Interfaces;

public interface IUpgradeRequestService
{
    Task<UpgradeRequestResponse?> RequestUpgradeAsync(
        Guid tenantId,
        Guid planId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UpgradeRequestResponse>> GetTenantRequestsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<UpgradeRequestResponse>> GetPendingRequestsPagedAsync(
        int page,
        int pageSize,
        string? status,
        CancellationToken cancellationToken = default);

    Task<UpgradeRequestResponse?> ApproveRequestAsync(
        Guid requestId,
        Guid reviewerUserId,
        CancellationToken cancellationToken = default);

    Task<UpgradeRequestResponse?> RejectRequestAsync(
        Guid requestId,
        Guid reviewerUserId,
        string? reason,
        CancellationToken cancellationToken = default);
}
