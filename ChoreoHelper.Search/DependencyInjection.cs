using ChoreoHelper.Search.Algorithms;
using ChoreoHelper.Search.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.Search;

public static class DependencyInjection
{
    public static IServiceCollection AddSearch(this IServiceCollection services)
    {
        services.AddTransient<IBehavior<SearchViewModel>, FilterOptionalFiguresBehavior>();
        services.AddTransient<IBehavior<SearchViewModel>, FilterRequiredFiguresBehavior>();
        services.AddTransient<IBehavior<SearchViewModel>, FindChoreographyBehavior>();
        services.AddTransient<IBehavior<SearchViewModel>, LoadDanceLevelsBehavior>();
        services.AddTransient<IBehavior<SearchViewModel>, LoadDancesBehavior>();
        services.AddTransient<IBehavior<SearchViewModel>, LoadFiguresBehavior>();
        services.AddTransient<IBehavior<SearchViewModel>, LoadOptionalFiguresBehavior>();
        services.AddTransient<IBehavior<SearchViewModel>, LoadRequiredFiguresBehavior>();
        services.AddTransient<IBehavior<SearchViewModel>, LoadSelectedFiguresBehavior>();
        services.AddTransient<IViewFor<SearchViewModel>, SearchView>();
        services.AddTransient<SearchViewModel>();
        services.AddTransient<ISearchChoreographies, SearchChoreographies>();
        return services;
    }
}