namespace OzoneMobileService.Application.Interfaces;

public interface ITenantContext
{
  /// <summary>Current tenant from JWT. Null for platform super admin.</summary>
  Guid? TenantId { get; }

  bool IsPlatformAdmin { get; }

  bool HasTenant => TenantId.HasValue;
}
