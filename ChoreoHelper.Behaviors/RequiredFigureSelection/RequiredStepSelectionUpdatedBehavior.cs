using ChoreoHelper.Messages;

namespace ChoreoHelper.Behaviors.RequiredFigureSelection;

public sealed class RequiredStepSelectionUpdatedBehavior : IBehavior<RequiredFigureSelectionViewModel>
{
    public void Activate(RequiredFigureSelectionViewModel viewModel, CompositeDisposable disposables)
    {
        viewModel
            .WhenAnyValue(vm => vm.IsSelected)
            .Select(_ => viewModel)
            .Subscribe(vm =>
            {
                var message = new RequiredFigureUpdated(vm.Hash, vm.IsSelected);
                MessageBus.Current.SendMessage(message);
            })
            .DisposeWith(disposables);
    }
}