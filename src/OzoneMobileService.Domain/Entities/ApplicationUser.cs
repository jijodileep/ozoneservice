using Microsoft.AspNetCore.Identity;

namespace OzoneMobileService.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>Null for platform super admin only.</summary>
    public Guid? TenantId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
