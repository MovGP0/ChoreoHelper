using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Splat;

namespace ReactiveUI;

public static class ReactiveObjectExtensions
{
    [UsedImplicitly]
    public static IDisposable Scoped<T>(this ReactiveObject obj, out T context)
        where T:class
    {
        var scopeFactory = Locator.Current.GetRequiredService<IServiceProvider>();
        var scope = scopeFactory.CreateScope();
        var services = scope.ServiceProvider;
        Debug.Assert(services is not null);
        context = services.GetRequiredService<T>();
        return scope;
    }

    [UsedImplicitly]
    public static bool IsInDesignMode(this ReactiveObject obj)
    {
        var value = (bool)DesignerProperties.IsInDesignModeProperty
            .GetMetadata(typeof(DependencyObject))
            .DefaultValue;

        return value != false;
    }
}