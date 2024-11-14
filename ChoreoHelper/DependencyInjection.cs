using ChoreoHelper.Behaviors;
using ChoreoHelper.Controls;
using ChoreoHelper.Graph;
using ChoreoHelper.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper;

public static class DependencyInjection
{
    public static IServiceCollection AddChoreoHelper(this IServiceCollection services)
    {
        services.AddMessagePipe();
        services.AddChoreoHelperDatabase();
        services.AddBehaviors();
        services.AddChoreoHelperControls();
        services.AddViewModels();
        return services;
    }
}