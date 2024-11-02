using ChoreoHelper.Editor.Behaviors;
using ChoreoHelper.Editor.Business;
using ChoreoHelper.Editor.ViewModels;
using ChoreoHelper.Editor.Views;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace ChoreoHelper.Editor;

public static class DependencyInjection
{
    public static IServiceCollection AddChoreoHelperEditor(this IServiceCollection services)
    {
        services.AddSingleton<ShellViewModel>();
        services.AddSingleton<IScreen, ShellViewModel>(r => r.GetRequiredService<ShellViewModel>());
        services.AddSingleton<IViewFor<TransitionEditorViewModel>, TransitionEditorControl>();
        services.AddSingleton<IViewFor<ShellViewModel>, ShellWindow>();
        services.AddSingleton<IBehavior<ShellViewModel>, OpenFileBehavior>();
        services.AddSingleton<IBehavior<ShellViewModel>, NavigateToTransitionEditorBehavior>();
        services.AddSingleton<GridPainter>();
        services.AddSingleton<Theme>();
        services.AddSingleton<TransitionEditorViewModel>();
        services.AddSingleton<XmlDataLoader>();
        services.AddSingleton<XmlDataSaver>();
        return services;
    }
}