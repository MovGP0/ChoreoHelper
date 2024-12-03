using ChoreoHelper.Messages;

namespace ChoreoHelper.OptionalFigureSelection.Behaviors;

public sealed class OptionalStepSelectionUpdatedBehavior(
    IPublisher<OptionalFigureUpdated> optionalFigureUpdatedPublisher)
    : IBehavior<OptionalFigureSelectionViewModel>
{
    public void Activate(OptionalFigureSelectionViewModel viewModel, CompositeDisposable disposables)
    {
        viewModel
            .WhenAnyValue(vm => vm.IsSelected)
            .Select(_ => viewModel)
            .Subscribe(vm =>
            {
                var message = new OptionalFigureUpdated(vm.Name, vm.IsSelected);
                optionalFigureUpdatedPublisher.Publish(message);
            })
            .DisposeWith(disposables);
    }
}