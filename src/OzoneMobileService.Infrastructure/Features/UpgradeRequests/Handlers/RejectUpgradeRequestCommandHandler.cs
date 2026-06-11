using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Subscription;
using OzoneMobileService.Application.Features.UpgradeRequests.Commands;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Features.UpgradeRequests.Handlers;

internal sealed class RejectUpgradeRequestCommandHandler(
    AppDbContext dbContext,
    NotificationWriter notificationWriter)
    : IRequestHandler<RejectUpgradeRequestCommand, UpgradeRequestResponse?>
{
    public async Task<UpgradeRequestResponse?> Handle(
        RejectUpgradeRequestCommand command,
        CancellationToken cancellationToken)
    {
        var upgradeRequest = await dbContext.SubscriptionUpgradeRequests
            .IgnoreQueryFilters()
            .Include(r => r.RequestedPlan)
            .FirstOrDefaultAsync(r => r.Id == command.RequestId, cancellationToken);

        if (upgradeRequest is null || upgradeRequest.Status != UpgradeRequestStatuses.Pending)
        {
            return null;
        }

        var tenant = await dbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == upgradeRequest.TenantId, cancellationToken);

        if (tenant is null)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        upgradeRequest.Status = UpgradeRequestStatuses.Rejected;
        upgradeRequest.ReviewedAt = now;
        upgradeRequest.ReviewedByUserId = command.ReviewerUserId;
        upgradeRequest.RejectionReason = command.Reason?.Trim();
        upgradeRequest.UpdatedAt = now;

        await dbContext.SaveChangesAsync(cancellationToken);

        await notificationWriter.CreateAsync(
            tenant.Id,
            Roles.TenantAdmin,
            "Upgrade rejected",
            $"Your request to upgrade to {upgradeRequest.RequestedPlan.Name} was rejected.{(string.IsNullOrWhiteSpace(command.Reason) ? "" : $" Reason: {command.Reason}")}",
            NotificationTypes.UpgradeRejected,
            cancellationToken);

        return await GetRequestByIdAsync(upgradeRequest.Id, cancellationToken);
    }

    private async Task<UpgradeRequestResponse?> GetRequestByIdAsync(
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var item = await GetTenantUpgradeRequestsQueryHandler.LoadRequestsQuery(dbContext)
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

        return item is null ? null : UpgradeRequestMapper.Map(item);
    }
}
