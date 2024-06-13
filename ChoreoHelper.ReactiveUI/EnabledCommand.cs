using System.Reactive;
using System.Reactive.Linq;
using JetBrains.Annotations;

namespace ReactiveUI;

///<summary>Use this to set commands when in Design Mode</summary>
public static class EnabledCommand
{
    [UsedImplicitly]
    public static readonly ReactiveCommand<Unit, Unit> Instance
        = ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default, Observable.Return(true));
}