using ChoreoHelper.Messages;

namespace ChoreoHelper.Behaviors.StepSelection;

public sealed class RequiredStepSelectionUpdatedBehavior : IBehavior<RequiredFigureSelectionViewModel>
{
    public void Activate(RequiredFigureSelectionViewModel viewModel, CompositeDisposable disposables)
    {
        viewModel
            .WhenAnyValue(vm => vm.IsSelected)
            .Select(_ => viewModel)
            .Subscribe(_ =>
            {
                MessageBus.Current.SendMessage(new RequiredFigureUpdated(viewModel.Hash));
            })
            .DisposeWith(disposables);
    }
}