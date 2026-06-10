namespace OzoneMobileService.Shared;

public static class SeedConstants
{
    public static readonly Guid StarterPlanId = Guid.Parse("00000000-0000-0000-0001-000000000001");
    public static readonly Guid ProPlanId = Guid.Parse("00000000-0000-0000-0001-000000000002");
    public static readonly Guid EnterprisePlanId = Guid.Parse("00000000-0000-0000-0001-000000000003");

    public static readonly Guid DevTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public const string DevAdminEmail = "admin@localhost.dev";
    public const string DevAdminPassword = "Admin@123";
    public const string DevAdminDisplayName = "Dev Admin";

    public const string SuperAdminEmail = "superadmin@localhost.dev";
    public const string SuperAdminPassword = "Super@123";
    public const string SuperAdminDisplayName = "Platform Super Admin";
}
