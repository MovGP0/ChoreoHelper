using ChoreoHelper.EditFigure.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.EditFigure;

public static class DependencyInjection
{
    public static IServiceCollection AddEditFigure(this IServiceCollection services)
    {
        services.AddTransient<EditFigureViewModel>();
        services.AddTransient<IViewFor<EditFigureViewModel>, EditFigureView>();
        services.AddTransient<IBehavior<EditFigureViewModel>, NavigateBackBehavior>();
        services.AddTransient<IBehavior<EditFigureViewModel>, SaveAndNavigateBackBehavior>();
        return services;
    }
}