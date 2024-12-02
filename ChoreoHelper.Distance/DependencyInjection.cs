using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.Distance;

public static class DependencyInjection
{
    public static IServiceCollection AddDistance(this IServiceCollection services)
    {
        services.AddTransient<DistanceViewModel>();
        return services;
    }
}