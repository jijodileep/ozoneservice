using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OzoneMobileService.Application.Common.Behaviors;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Infrastructure.Authorization;
using OzoneMobileService.Infrastructure.Features.Auth;
using OzoneMobileService.Infrastructure.Features.Branches;
using OzoneMobileService.Infrastructure.Features.UpgradeRequests;
using OzoneMobileService.Infrastructure.Features.Notifications;
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
        services.AddScoped<IBranchContext, BranchContext>();

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddScoped<BranchAccessService>();
        services.AddScoped<NotificationWriter>();
        services.AddScoped<AuthTokenIssuer>();
        services.AddScoped<SubscriptionExpiryChecker>();

        services.AddIdentityServices(configuration);

        return services;
    }
}
