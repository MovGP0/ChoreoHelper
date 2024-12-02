﻿using ChoreoHelper.Messages;

namespace ChoreoHelper.MainWindow.Behaviors;

public sealed class CloseDrawerBehavior(ISubscriber<CloseDrawer> closeDrawerSubscriber)
    : IBehavior<MainWindowViewModel>
{
    public void Activate(MainWindowViewModel viewModel, CompositeDisposable disposables)
    {
        closeDrawerSubscriber
            .Subscribe(_ => viewModel.IsDrawerOpen = false)
            .DisposeWith(disposables);
    }
}