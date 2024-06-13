using ChoreoHelper.ViewModels;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace ChoreoHelper.Controls;

public static class DependencyInjection
{
    [UsedImplicitly]
    public static IServiceCollection AddWpfControls(this IServiceCollection services)
    {
        services.AddTransient<IViewFor<MainWindowViewModel>, MainWindow>();
        services.AddTransient<IViewFor<RequiredFigureSelectionViewModel>, RequiredFigureSelection>();
        services.AddTransient<IViewFor<OptionalFigureSelectionViewModel>, OptionalFigureSelection>();
        services.AddTransient<IViewFor<ChoreographyViewModel>, Choreography>();
        services.AddTransient<IViewFor<LevelSelectionViewModel>, LevelSelection>();
        return services;
    }
}