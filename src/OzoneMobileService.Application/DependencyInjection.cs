using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OzoneMobileService.Application.Features.Branches.Commands;

namespace OzoneMobileService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateBranchCommandValidator>();
        return services;
    }
}
