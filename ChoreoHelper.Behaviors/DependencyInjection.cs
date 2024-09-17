using ChoreoHelper.Behaviors.Choreography;
using ChoreoHelper.Behaviors.LevelSelection;
using ChoreoHelper.Behaviors.MainWindow;
using ChoreoHelper.Behaviors.OptionalFigureSelection;
using ChoreoHelper.Behaviors.RequiredFigureSelection;
using ChoreoHelper.Behaviors.Search;
using ChoreoHelper.Behaviors.SearchResult;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.Behaviors;

public static class DependencyInjection
{
    public static IServiceCollection AddBehaviors(this IServiceCollection services)
    {
        services.AddTransient<IBehavior<ChoreographyViewModel>, CopyBehavior>();
        services.AddTransient<IBehavior<LevelSelectionViewModel>, LevelSelectionChangedBehavior>();
        services.AddTransient<IBehavior<LevelSelectionViewModel>, UpdateDanceLevelNameBehavior>();
        services.AddTransient<IBehavior<MainWindowViewModel>, CloseDrawerBehavior>();
        services.AddTransient<IBehavior<OptionalFigureSelectionViewModel>, OptionalStepSelectionUpdatedBehavior>();
        services.AddTransient<IBehavior<RequiredFigureSelectionViewModel>, RequiredStepSelectionUpdatedBehavior>();
        services.AddTransient<IBehavior<SearchResultViewModel>, CloseDrawerOnChoreographiesFoundBehavior>();
        services.AddTransient<IBehavior<SearchResultViewModel>, LoadChoreographyBehavior>();
        services.AddTransient<IBehavior<SearchViewModel>, FilterOptionalFiguresBehavior>();
        services.AddTransient<IBehavior<SearchViewModel>, FilterRequiredFiguresBehavior>();
        services.AddTransient<IBehavior<SearchViewModel>, FindChoreographyBehavior>();
        services.AddTransient<IBehavior<SearchViewModel>, LoadDanceLevelsBehavior>();
        services.AddTransient<IBehavior<SearchViewModel>, LoadDancesBehavior>();
        services.AddTransient<IBehavior<SearchViewModel>, LoadFiguresBehavior>();
        services.AddTransient<IBehavior<SearchViewModel>, LoadOptionalFiguresBehavior>();
        services.AddTransient<IBehavior<SearchViewModel>, LoadRequiredFiguresBehavior>();
        services.AddTransient<IBehavior<SearchViewModel>, LoadSelectedFiguresBehavior>();
        return services;
    }
}