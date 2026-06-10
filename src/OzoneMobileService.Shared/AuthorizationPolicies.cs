namespace OzoneMobileService.Shared;

/// <summary>
/// Authorization policy names and role matrix for the API.
/// </summary>
/// <remarks>
/// | Policy            | TenantAdmin | ShopAdmin | ShopStaff | Accountant | PlatformSuperAdmin |
/// |-------------------|:-----------:|:---------:|:---------:|:----------:|:------------------:|
/// | SetupWrite        |     Yes     |    Yes    |    No     |     No     |         No         |
/// | OperationalWrite  |     No      |    Yes    |    Yes    |     No     |         No         |
/// | ReportsRead       |     Yes     |    Yes    |    No     |    Yes     |         No         |
/// | PlatformSuperAdmin|     No      |    No     |    No     |     No     |        Yes         |
///
/// PlatformSuperAdmin bypasses tenant filters and may only use /api/platform/* (plus auth/health).
/// Operational writes are intended for mobile (Flutter); setup writes for web (Angular).
/// </remarks>
public static class AuthorizationPolicies
{
    public const string PlatformSuperAdmin = "PlatformSuperAdmin";
    public const string SetupWrite = "SetupWrite";
    public const string OperationalWrite = "OperationalWrite";
    public const string ReportsRead = "ReportsRead";
}
