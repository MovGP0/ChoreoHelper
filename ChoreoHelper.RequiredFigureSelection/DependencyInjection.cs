using ChoreoHelper.RequiredFigureSelection.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.RequiredFigureSelection;

public static class DependencyInjection
{
    public static IServiceCollection AddRequiredFigureSelection(this IServiceCollection services)
    {
        services.AddTransient<IBehavior<RequiredFigureSelectionViewModel>, SetColorBasedOnLevelBehavior>();
        services.AddTransient<IBehavior<RequiredFigureSelectionViewModel>, RequiredStepSelectionUpdatedBehavior>();
        services.AddTransient<IViewFor<RequiredFigureSelectionViewModel>, RequiredFigureSelectionView>();
        services.AddTransient<RequiredFigureSelectionViewModel>();
        return services;
    }
}