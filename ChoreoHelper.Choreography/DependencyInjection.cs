using ChoreoHelper.Choreography.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.Choreography;

public static class DependencyInjection
{
    public static IServiceCollection AddChoreography(this IServiceCollection services)
    {
        services.AddTransient<IBehavior<ChoreographyViewModel>, CopyBehavior>();
        services.AddTransient<IViewFor<ChoreographyViewModel>, ChoreographyView>();
        services.AddTransient<ChoreographyViewModel>();
        return services;
    }
}