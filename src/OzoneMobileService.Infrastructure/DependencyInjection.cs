using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Infrastructure.Identity;
using OzoneMobileService.Infrastructure.MultiTenancy;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, TenantContext>();

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddIdentityServices(configuration);

        return services;
    }
}
