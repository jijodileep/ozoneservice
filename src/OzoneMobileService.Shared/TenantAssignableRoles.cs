namespace OzoneMobileService.Shared;

public static class TenantAssignableRoles
{
    public static readonly string[] All =
    [
        Roles.ShopAdmin,
        Roles.ShopStaff,
        Roles.Accountant
    ];

    public static bool IsAssignable(string role) =>
        All.Contains(role, StringComparer.Ordinal);
}
