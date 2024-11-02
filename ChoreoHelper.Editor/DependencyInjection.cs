using ChoreoHelper.Editor.Shared;
using ChoreoHelper.Editor.Shell;
using ChoreoHelper.Editor.TransitionEditor;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.Editor;

public static class DependencyInjection
{
    public static IServiceCollection AddChoreoHelperEditor(this IServiceCollection services)
    {
        services.AddMessagePipe();
        services.AddTransitionEditor();
        services.AddShell();
        services.AddSingleton<Theme>();
        services.AddSingleton<XmlDataLoader>();
        services.AddSingleton<XmlDataSaver>();
        return services;
    }
}