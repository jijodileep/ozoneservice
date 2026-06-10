namespace OzoneMobileService.Application.Interfaces;

public interface ISubscriptionLimitService
{
    Task ValidateCanAddUserAsync(Guid tenantId, CancellationToken cancellationToken = default);

    Task ValidateCanAddBranchAsync(Guid tenantId, CancellationToken cancellationToken = default);

    Task ValidatePlanAssignmentAsync(
        Guid tenantId,
        Guid planId,
        CancellationToken cancellationToken = default);
}
