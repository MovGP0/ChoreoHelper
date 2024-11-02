using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChoreoHelper.Editor.TransitionEditor.Events;
using DynamicData;
using DynamicData.Kernel;
using MessagePipe;
using ReactiveUI;

namespace ChoreoHelper.Editor.TransitionEditor.Behaviors;

/// <summary>
/// Loads the figures when a dance was selected.
/// </summary>
public sealed class DanceSelectedBehavior(IPublisher<RenderTransitionEditorCommand> publisher) : IBehavior<TransitionEditorViewModel>
{
    public void Activate(TransitionEditorViewModel viewModel, CompositeDisposable disposables)
    {
        viewModel
            .WhenAnyValue(vm => vm.SelectedDance)
            .Select(_ => viewModel)
            .Subscribe(vm =>
            {
                vm.Figures.Clear();
                if (vm.SelectedDance is null)
                {
                    return;
                }

                vm.Figures.AddRange(vm.SelectedDance.Figures);

                var numberOfFigures = vm.SelectedDance.Figures.Count;

                var transitionMatrix = new byte[numberOfFigures, numberOfFigures];
                for (var column = 0; column < numberOfFigures; column++)
                for (var row = 0; row < numberOfFigures; row++)
                {
                    var source = vm.SelectedDance.Figures[column];
                    var target = vm.SelectedDance.Figures[row];
                    var transition = vm.SelectedDance.Transitions
                        .FirstOrOptional(t => t.Source == source && t.Target == target);

                    transitionMatrix[column, row] = transition.HasValue
                        ? (byte)transition.Value.Distance
                        : (byte)0;
                }

                vm.Transitions = transitionMatrix;

                publisher.Publish(new RenderTransitionEditorCommand());
            })
            .DisposeWith(disposables);
    }
}