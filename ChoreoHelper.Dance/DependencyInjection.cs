using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.Dance;

public static class DependencyInjection
{
    public static IServiceCollection AddDance(this IServiceCollection services)
    {
        services.AddTransient<DanceViewModel>();
        return services;
    }
}