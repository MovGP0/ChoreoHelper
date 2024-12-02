using ChoreoHelper.LevelSelection.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.LevelSelection;

public static class DependencyInjection
{
    public static IServiceCollection AddLevelSelection(this IServiceCollection services)
    {
        services.AddTransient<IBehavior<LevelSelectionViewModel>, LevelSelectionChangedBehavior>();
        services.AddTransient<IBehavior<LevelSelectionViewModel>, UpdateDanceLevelNameBehavior>();
        services.AddTransient<LevelSelectionViewModel>();
        services.AddTransient<IViewFor<LevelSelectionViewModel>, LevelSelectionView>();
        return services;
    }
}