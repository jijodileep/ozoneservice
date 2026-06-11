using OzoneMobileService.Application.DTOs.Subscription;
using OzoneMobileService.Domain.Entities;

namespace OzoneMobileService.Infrastructure.Features.UpgradeRequests;

internal static class UpgradeRequestMapper
{
    internal static UpgradeRequestResponse Map(SubscriptionUpgradeRequest request) =>
        new(
            request.Id,
            request.TenantId,
            request.Tenant.Name,
            request.CurrentPlan.Name,
            request.RequestedPlan.Name,
            request.RequestedPlan.Price,
            request.Status,
            request.RequestedAt,
            request.ReviewedAt,
            request.RejectionReason,
            request.InvoiceId,
            request.Invoice?.InvoiceNumber);
}
