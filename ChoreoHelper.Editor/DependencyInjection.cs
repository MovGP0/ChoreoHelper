using ChoreoHelper.Editor.Behaviors;
using ChoreoHelper.Editor.Business;
using ChoreoHelper.Editor.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace ChoreoHelper.Editor;

public static class DependencyInjection
{
    public static IServiceCollection AddChoreoHelperEditor(this IServiceCollection services)
    {
        services.AddTransient<IViewFor<MainViewModel>, MainWindow>();
        services.AddTransient<GridPainter>();
        services.AddTransient<Theme>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<IBehavior<MainViewModel>, OpenFileBehavior>();
        services.AddTransient<XmlDataLoader>();
        services.AddTransient<XmlDataSaver>();
        return services;
    }
}