using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI.Batching;

namespace ReactiveUI.TestObjects;

public static class BatchingReactiveObjectExtensions
{
    public static IObservable<Unit> WhenAnyValueBatched<TSender>(
        this TSender sender,
        params string[] propertyNames)
        where TSender : IBatchingReactiveObject
    {
        var propertyNamesSet = propertyNames
            .ToImmutableHashSet();

        return CreateAnyPropertyChangedObservable(sender, propertyNamesSet);
    }

    public static IObservable<Unit> WhenAnyValueBatched<TSender>(
        this TSender sender,
        params Expression<Func<TSender, object?>>[] propertyExpressions)
        where TSender : IBatchingReactiveObject
    {
        var propertyNamesSet = propertyExpressions
            .Select(PropertyPathResolver.GetPropertyChain)
            .ToImmutableHashSet();

        return CreateAnyPropertyChangedObservable(sender, propertyNamesSet);
    }

    private static IObservable<Unit> CreateAnyPropertyChangedObservable<TSender>(TSender sender, ImmutableHashSet<string> propertyNamesSet) where TSender : IBatchingReactiveObject
    {
        return Observable.Create<Unit>(observer =>
        {
            EventHandler<BatchPropertyChangedEventArgs> handler = (s, e) =>
            {
                if (e is not { } batchArgs)
                {
                    return;
                }

                // Check if any of the changed properties match the provided expressions
                var changedChains = batchArgs.ChangedProperties
                    .Any(propertyNamesSet.Contains);

                if (changedChains)
                {
                    observer.OnNext(Unit.Default);
                }
            };

            sender.PropertiesChanged += handler;
            return Disposable.Create(() => sender.PropertiesChanged -= handler);
        });
    }

    public static IDisposable ObserveChildPropertyChanges<TParent, TChild>(
        this TParent parent,
        Expression<Func<TParent, TChild>> childPropertyExpression)
        where TParent : IReactiveObject
        where TChild : INotifyPropertyChanged
    {
        if (parent is null) throw new ArgumentNullException(nameof(parent));
        if (childPropertyExpression is null) throw new ArgumentNullException(nameof(childPropertyExpression));

        var childPropertyName = GetPropertyName(childPropertyExpression);
        var childPropertyFunc = childPropertyExpression.Compile();

        var serialDisposable = new SerialDisposable();

        // Observe when the child property changes
        var childPropertyChangedSubscription = parent.WhenAnyValue(childPropertyExpression)
            .Subscribe(_ => UpdateChildSubscription());

        // Initial subscription to the child
        UpdateChildSubscription();

        return new CompositeDisposable(serialDisposable, childPropertyChangedSubscription);

        void UpdateChildSubscription()
        {
            var child = childPropertyFunc(parent);

            if (child is null)
            {
                serialDisposable.Disposable = null;
                return;
            }

            serialDisposable.Disposable = Observable
                .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => child.PropertyChanged += h,
                    h => child.PropertyChanged -= h)
                .Subscribe(a =>
                {
                    var args = a.EventArgs;
                    // Raise PropertyChanged on parent
                    var propertyName = $"{childPropertyName}.{args.PropertyName}";
                    parent.RaisePropertyChanged(propertyName);
                });
        }
    }

    private static string GetPropertyName<T, TProp>(Expression<Func<T, TProp>> expression)
    {
        if (expression.Body is MemberExpression memberExpr)
        {
            return memberExpr.Member.Name;
        }

        if (expression.Body is UnaryExpression { Operand: MemberExpression memberExpr2 })
        {
            return memberExpr2.Member.Name;
        }

        throw new ArgumentException("Invalid expression", nameof(expression));
    }
}
