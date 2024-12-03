using System.Reactive;
using System.Reactive.Linq;
using JetBrains.Annotations;

namespace ReactiveUI;

/// <summary>
/// Use this for commands that are always enabled.
/// </summary>
public static class EnabledCommand
{
    /// <summary>
    /// A command that does nothing and is enabled by default.
    /// </summary>
    [UsedImplicitly]
    public static readonly ReactiveCommand<Unit, Unit> Instance
        = ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default, Observable.Return(true));
}