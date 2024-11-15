using ChoreoHelper.ViewModels;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace ChoreoHelper.Controls;

public static class DependencyInjection
{
    [UsedImplicitly]
    public static IServiceCollection AddChoreoHelperControls(this IServiceCollection services)
    {
        services.AddTransient<IViewFor<ChoreographyViewModel>, Choreography>();
        services.AddTransient<IViewFor<EditFigureViewModel>, EditFigureView>();
        services.AddTransient<IViewFor<LevelSelectionViewModel>, LevelSelection>();
        services.AddTransient<IViewFor<MainWindowViewModel>, MainWindow>();
        services.AddTransient<IViewFor<OptionalFigureSelectionViewModel>, OptionalFigureSelection>();
        services.AddTransient<IViewFor<RequiredFigureSelectionViewModel>, RequiredFigureSelection>();
        services.AddTransient<IViewFor<SearchResultViewModel>, SearchResultView>();
        services.AddTransient<IViewFor<TransitionViewModel>, TransitionView>();
        return services;
    }
}