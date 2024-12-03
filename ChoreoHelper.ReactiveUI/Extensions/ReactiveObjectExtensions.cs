using System.ComponentModel;
using System.Reactive.Disposables;
using System.Windows;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Splat;

namespace ReactiveUI.Extensions;

public static class ReactiveObjectExtensions
{
    [UsedImplicitly]
    public static IDisposable Scoped<T>(this ReactiveObject obj, out T context)
        where T:class
    {
        var scopeFactory = Locator.Current.GetRequiredService<IServiceProvider>();
        var scope = scopeFactory.CreateScope();
        var services = scope.ServiceProvider;
        context = services.GetRequiredService<T>();
        return scope;
    }

    [UsedImplicitly]
    public static bool IsInDesignMode(this ReactiveObject obj)
    {
        var value = DesignerProperties.IsInDesignModeProperty
            .GetMetadata(typeof(DependencyObject))
            .DefaultValue;

        return value is true;
    }

    public static void ActivateBehaviors<TViewModel>(this TViewModel viewModel, CompositeDisposable disposables)
        where TViewModel: ReactiveObject
    {
        foreach (var behavior in Locator.Current.GetServices<IBehavior<TViewModel>>())
        {
            behavior.Activate(viewModel, disposables);
        }
    }
}