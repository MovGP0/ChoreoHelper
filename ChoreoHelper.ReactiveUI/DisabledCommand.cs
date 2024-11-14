﻿using System.Reactive;
using System.Reactive.Linq;
using JetBrains.Annotations;

namespace ReactiveUI;

///<summary>Use this as a default value for commands</summary>
public static class DisabledCommand
{
    /// <summary>
    /// A command that does nothing and is disabled by default.
    /// </summary>
    /// <remarks>
    /// This command is enabled when the control is running in design mode.
    /// </remarks>
    [UsedImplicitly]
    public static readonly ReactiveCommand<Unit, Unit> Instance
        = ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default, Observable.Return(new ReactiveObject().IsInDesignMode()));
}