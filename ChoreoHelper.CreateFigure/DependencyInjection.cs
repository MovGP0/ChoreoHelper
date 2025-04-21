using ChoreoHelper.CreateFigure.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.CreateFigure;

public static class DependencyInjection
{
    public static IServiceCollection AddCreateFigure(this IServiceCollection services)
    {
        services.AddTransient<CreateFigureViewModel>();
        services.AddTransient<IViewFor<CreateFigureViewModel>, CreateFigureView>();
        services.AddTransient<IBehavior<CreateFigureViewModel>, NavigateBackBehavior>();
        services.AddTransient<IBehavior<CreateFigureViewModel>, SaveAndNavigateBackBehavior>();
        return services;
    }
}
