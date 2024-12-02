using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.Transition;

public static class DependencyInjection
{
    public static IServiceCollection AddTransition(this IServiceCollection services)
    {
        services.AddTransient<IViewFor<TransitionViewModel>, TransitionView>();
        services.AddTransient<TransitionViewModel>();
        return services;
    }
}