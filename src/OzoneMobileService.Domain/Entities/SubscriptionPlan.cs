namespace OzoneMobileService.Domain.Entities;

public class SubscriptionPlan
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public int MaxUsers { get; set; }

    public int MaxBranches { get; set; }

    public int MaxDevicesPerUser { get; set; } = 1;

    public bool AllowWebLogin { get; set; } = true;

    public bool AllowMobileLogin { get; set; } = true;

    public bool IsActive { get; set; } = true;

    /// <summary>Price per billing period (INR).</summary>
    public decimal Price { get; set; }

    public int BillingPeriodMonths { get; set; } = 1;

    /// <summary>Higher tier allows upgrade only to plans with greater tier order.</summary>
    public int TierOrder { get; set; } = 1;
}
