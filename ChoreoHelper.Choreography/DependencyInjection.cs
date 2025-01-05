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

        services.AddTransient<IBehavior<ChoreographyItemViewModel>, SetColorBasedOnLevelBehavior>();
        services.AddTransient<IViewFor<ChoreographyItemViewModel>, ChoreographyItemView>();
        services.AddTransient<ChoreographyItemViewModel>();
        return services;
    }
}