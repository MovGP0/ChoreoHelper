using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.Algorithms;

public static class DependencyInjection
{
    public static IServiceCollection AddAlgorithms(this IServiceCollection services)
    {
        services.AddSingleton<IUnreachableIslandsFinder, DepthFirstSearchUnreachableIslandsFinder>();
        services.AddSingleton<IRouteFinder, BreadthFirstSearchRouteFinder>();
        return services;
    }
}