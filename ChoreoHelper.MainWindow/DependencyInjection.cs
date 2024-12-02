using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.MainWindow;

public static class DependencyInjection
{
    public static IServiceCollection AddMainWindow(this IServiceCollection services)
    {
        services.AddTransient<IBehavior<MainWindow>>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<IViewFor<MainWindowViewModel>, MainWindow>();
        return services;
    }
}