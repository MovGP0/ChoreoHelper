using ChoreoHelper.Transition.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.Transition;

public static class DependencyInjection
{
    public static IServiceCollection AddTransition(this IServiceCollection services)
    {
        services.AddTransient<IViewFor<TransitionViewModel>, TransitionView>();
        services.AddTransient<TransitionViewModel>();
        services.AddTransient<IBehavior<TransitionViewModel>, NavigateBackBehavior>();
        services.AddTransient<IBehavior<TransitionViewModel>, SaveAndNavigateBackBehavior>();
        return services;
    }
}