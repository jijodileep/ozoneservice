namespace OzoneMobileService.Domain.Common;

public abstract class BaseEntity : ITenantEntity
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
