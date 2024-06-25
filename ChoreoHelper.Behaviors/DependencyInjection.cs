using ChoreoHelper.Behaviors.Choreography;
using ChoreoHelper.Behaviors.LevelSelection;
using ChoreoHelper.Behaviors.MainWindow;
using ChoreoHelper.Behaviors.StepSelection;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.Behaviors;

public static class DependencyInjection
{
    public static IServiceCollection AddBehaviors(this IServiceCollection services)
    {
        services.AddTransient<IBehavior<MainWindowViewModel>, LoadDancesBehavior>();
        services.AddTransient<IBehavior<MainWindowViewModel>, LoadRequiredFiguresBehavior>();
        services.AddTransient<IBehavior<MainWindowViewModel>, LoadOptionalFiguresBehavior>();
        services.AddTransient<IBehavior<MainWindowViewModel>, FindChoreographyBehavior>();
        services.AddTransient<IBehavior<MainWindowViewModel>, LoadDanceLevelsBehavior>();
        services.AddTransient<IBehavior<MainWindowViewModel>, CloseDrawerOnChoreographiesFound>();
        services.AddTransient<IBehavior<RequiredFigureSelectionViewModel>, RequiredStepSelectionUpdatedBehavior>();
        services.AddTransient<IBehavior<LevelSelectionViewModel>, UpdateDanceLevelNameBehavior>();
        services.AddTransient<IBehavior<LevelSelectionViewModel>, LevelSelectionChangedBehavior>();
        services.AddTransient<IBehavior<ChoreographyViewModel>, CopyBehavior>();
        return services;
    }
}