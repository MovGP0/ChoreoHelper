using ChoreoHelper.SearchResult.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.SearchResult;

public static class DependencyInjection
{
    public static IServiceCollection AddSearchResult(this IServiceCollection services)
    {
        services.AddTransient<IBehavior<SearchResultViewModel>, LoadChoreographyBehavior>();
        services.AddTransient<IViewFor<SearchResultViewModel>, SearchResultView>();
        services.AddScoped<SearchResultViewModel>();
        return services;
    }
}