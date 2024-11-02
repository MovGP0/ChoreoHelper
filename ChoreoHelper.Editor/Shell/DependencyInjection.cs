using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace ChoreoHelper.Editor.Shell;

public static class DependencyInjection
{
    public static IServiceCollection AddShell(this IServiceCollection services)
    {
        return services
            .AddSingleton<ShellViewModel>()
            .AddSingleton<IScreen, ShellViewModel>(r => r.GetRequiredService<ShellViewModel>())
            .AddSingleton<IViewFor<ShellViewModel>, ShellWindow>()
            .AddSingleton<IBehavior<ShellViewModel>, OpenFileBehavior>()
            .AddSingleton<IBehavior<ShellViewModel>, NavigateToTransitionEditorBehavior>();
    }
}