using OzoneMobileService.Domain.Common;

namespace OzoneMobileService.Domain.Entities;

public class CustomerDevice : BaseEntity
{
    public Guid CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    public Guid VariantId { get; set; }

    public MobileVariant Variant { get; set; } = null!;

    public string? Imei { get; set; }

    public Guid RegisteredAtBranchId { get; set; }

    public Branch RegisteredAtBranch { get; set; } = null!;

    public bool IsActive { get; set; } = true;
}
