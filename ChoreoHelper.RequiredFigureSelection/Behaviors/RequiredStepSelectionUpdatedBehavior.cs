﻿using ChoreoHelper.Messages;

namespace ChoreoHelper.RequiredFigureSelection.Behaviors;

public sealed class RequiredStepSelectionUpdatedBehavior(
    IPublisher<RequiredFigureUpdated> requiredFigureUpdatedPublisher)
    : IBehavior<RequiredFigureSelectionViewModel>
{
    public void Activate(RequiredFigureSelectionViewModel viewModel, CompositeDisposable disposables)
    {
        viewModel
            .WhenAnyValue(vm => vm.IsSelected)
            .Select(_ => viewModel)
            .Subscribe(vm =>
            {
                var message = new RequiredFigureUpdated(vm.Name, vm.IsSelected);
                requiredFigureUpdatedPublisher.Publish(message);
            })
            .DisposeWith(disposables);
    }
}