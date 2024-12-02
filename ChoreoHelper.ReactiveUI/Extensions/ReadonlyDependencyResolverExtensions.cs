using Splat;

namespace ReactiveUI.Extensions;

public static class ReadonlyDependencyResolverExtensions
{
    public static T GetRequiredService<T>(this IReadonlyDependencyResolver resolver, string? contract = null)
    {
        ArgumentNullException.ThrowIfNull(resolver);

        var service = (T?)resolver.GetService(typeof(T), contract);
        if (service is null)
        {
            throw new InvalidOperationException($"Service of type {typeof(T)} was not found.");
        }

        return service;
    }
}