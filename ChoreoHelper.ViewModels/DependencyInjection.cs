using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.ViewModels;

public static class DependencyInjection
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddTransient<LevelSelectionViewModel>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<ChoreographyViewModel>();
        services.AddTransient<OptionalFigureSelectionViewModel>();
        services.AddTransient<RequiredFigureSelectionViewModel>();
        return services;
    }
}