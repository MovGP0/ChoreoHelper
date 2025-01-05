using ChoreoHelper.OptionalFigureSelection.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.OptionalFigureSelection;

public static class DependencyInjection
{
    public static IServiceCollection AddOptionalFigureSelection(this IServiceCollection services)
    {
        services.AddTransient<IBehavior<OptionalFigureSelectionViewModel>, SetColorBasedOnLevelBehavior>();
        services.AddTransient<IBehavior<OptionalFigureSelectionViewModel>, OptionalStepSelectionUpdatedBehavior>();
        services.AddTransient<IViewFor<OptionalFigureSelectionViewModel>, OptionalFigureSelectionView>();
        services.AddTransient<OptionalFigureSelectionViewModel>();
        return services;
    }
}