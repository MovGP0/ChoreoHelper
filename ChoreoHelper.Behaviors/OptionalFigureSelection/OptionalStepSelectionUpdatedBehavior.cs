using ChoreoHelper.Messages;

namespace ChoreoHelper.Behaviors.OptionalFigureSelection;

public sealed class OptionalStepSelectionUpdatedBehavior : IBehavior<OptionalFigureSelectionViewModel>
{
    public void Activate(OptionalFigureSelectionViewModel viewModel, CompositeDisposable disposables)
    {
        viewModel
            .WhenAnyValue(vm => vm.IsSelected)
            .Select(_ => viewModel)
            .Subscribe(vm =>
            {
                var message = new OptionalFigureUpdated(vm.Hash, vm.IsSelected);
                MessageBus.Current.SendMessage(message);
            })
            .DisposeWith(disposables);
    }
}