using ChoreHelper.Editor.Business;
using ChoreHelper.Editor.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace ChoreHelper.Editor;

public static class DependencyInjection
{
    public static IServiceCollection AddChoreoHelperEditor(this IServiceCollection services)
    {
        services.AddTransient<IViewFor<MainViewModel>, MainWindow>();
        services.AddTransient<GridPainter>();
        services.AddTransient<Theme>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<XmlDataLoader>();
        services.AddTransient<XmlDataSaver>();
        return services;
    }
}