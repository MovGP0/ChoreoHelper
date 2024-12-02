using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.Restriction;

public static class DependencyInjection
{
    public static IServiceCollection AddRestriction(this IServiceCollection services)
    {
        services.AddTransient<RestrictionViewModel>();
        return services;
    }
}