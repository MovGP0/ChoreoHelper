using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.ViewModels;

public static class DependencyInjection
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddTransient<ChoreographyViewModel>();
        services.AddTransient<DanceViewModel>();
        services.AddTransient<DistanceViewModel>();
        services.AddTransient<EditFigureViewModel>();
        services.AddTransient<FigureViewModel>();
        services.AddTransient<LevelSelectionViewModel>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<OptionalFigureSelectionViewModel>();
        services.AddTransient<RequiredFigureSelectionViewModel>();
        services.AddTransient<RestrictionViewModel>();
        services.AddTransient<SearchResultViewModel>();
        services.AddTransient<SearchViewModel>();
        services.AddTransient<TransitionViewModel>();
        return services;
    }
}