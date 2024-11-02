using ChoreoHelper.Editor.TransitionEditor.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace ChoreoHelper.Editor.TransitionEditor;

public static class DependencyInjection
{
    public static IServiceCollection AddTransitionEditor(this IServiceCollection services)
    {
        return services
            .AddSingleton<GridPainter>()
            .AddSingleton<TransitionEditorViewModel>()
            .AddSingleton<IViewFor<TransitionEditorViewModel>, TransitionEditorControl>()
            .AddSingleton<IBehavior<TransitionEditorViewModel>, DancesLoadedBehavior>()
            .AddSingleton<IBehavior<TransitionEditorViewModel>, DanceSelectedBehavior>();
    }
}