using ChoreoHelper.Behaviors;
using ChoreoHelper.Controls;
using ChoreoHelper.Database;
using ChoreoHelper.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper;

public static class DependencyInjection
{
    public static IServiceCollection AddChoreoHelper(this IServiceCollection services)
    {
        services.AddChoreoHelperDatabase();
        services.AddBehaviors();
        services.AddWpfControls();
        services.AddViewModels();
        return services;
    }
}