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
            .AddTransient<IViewFor<TransitionEditorViewModel>, TransitionEditorView>()
            .AddTransient<IBehavior<TransitionEditorViewModel>, DancesLoadedBehavior>()
            .AddTransient<IBehavior<TransitionEditorViewModel>, DanceSelectedBehavior>()
            .AddTransient<IBehavior<TransitionEditorViewModel>, ResetZoomBehavior>()
            .AddTransient<IBehavior<TransitionEditorViewModel>, CloseEditViewBehavior>();
    }
}