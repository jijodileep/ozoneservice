namespace OzoneMobileService.Domain.Entities;

public class AppNotification
{
    public Guid Id { get; set; }

    public Guid? TenantId { get; set; }

    public Tenant? Tenant { get; set; }

    /// <summary>Target role: PlatformSuperAdmin, TenantAdmin, etc.</summary>
    public string RoleTarget { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string NotificationType { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }
}
