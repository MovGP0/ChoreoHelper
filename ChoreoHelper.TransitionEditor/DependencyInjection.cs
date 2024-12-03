using ChoreoHelper.TransitionEditor.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.TransitionEditor;

public static class DependencyInjection
{
    public static IServiceCollection AddTransitionEditor(this IServiceCollection services)
    {
        return services
            .AddSingleton<Theme>()
            .AddSingleton<GridPainter>()
            .AddScoped<TransitionEditorViewModel>()
            .AddSingleton<IViewFor<TransitionEditorViewModel>, TransitionEditorView>()
            .AddSingleton<IBehavior<TransitionEditorViewModel>, DancesLoadedBehavior>()
            .AddSingleton<IBehavior<TransitionEditorViewModel>, DanceSelectedBehavior>()
            .AddSingleton<IBehavior<TransitionEditorViewModel>, ResetZoomBehavior>();
    }
}