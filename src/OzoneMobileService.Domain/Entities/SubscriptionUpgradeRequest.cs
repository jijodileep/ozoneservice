using OzoneMobileService.Domain.Common;

namespace OzoneMobileService.Domain.Entities;

public class SubscriptionUpgradeRequest : BaseEntity
{
    public Guid RequestedPlanId { get; set; }

    public SubscriptionPlan RequestedPlan { get; set; } = null!;

    public Guid CurrentPlanId { get; set; }

    public SubscriptionPlan CurrentPlan { get; set; } = null!;

    public Tenant Tenant { get; set; } = null!;

    public string Status { get; set; } = string.Empty;

    public DateTime RequestedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public Guid? ReviewedByUserId { get; set; }

    public string? RejectionReason { get; set; }

    public Guid? InvoiceId { get; set; }

    public Invoice? Invoice { get; set; }
}
