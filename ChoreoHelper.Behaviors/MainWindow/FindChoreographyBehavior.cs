using System.Runtime.CompilerServices;
using ChoreoHelper.Behaviors.Algorithms;
using ChoreoHelper.Behaviors.Extensions;
using ChoreoHelper.Messages;
using DynamicData.Kernel;

namespace ChoreoHelper.Behaviors.MainWindow;

public sealed class FindChoreographyBehavior(
    ISubscriber<RequiredFigureUpdated> requiredFigureUpdated,
    IPublisher<FoundChoreographies> foundChoreographies,
    ISearchChoreographies searchChoreographies)
    : IBehavior<SearchViewModel>
{
    public void Activate(SearchViewModel viewModel, CompositeDisposable disposables)
    {
        // at least two required figures are selected
        var obs0 = viewModel.RequiredFiguresFiltered
            .OnCollectionChanged()
            .Select(_ => Unit.Default);

        var obs1 = viewModel.OptionalFiguresFiltered
            .OnCollectionChanged()
            .Select(_ => Unit.Default);

        var obs2 = requiredFigureUpdated
            .AsObservable()
            .Do(message =>
            {
                var figure = viewModel.RequiredFigures
                    .FirstOrOptional(rf => rf.Hash == message.Hash);

                if (figure.HasValue)
                {
                    figure.Value.IsSelected = message.IsSelected;
                }
            })
            .Select(_ => Unit.Default);

        var obs3 = viewModel.WhenAnyValue(
                vm => vm.IsStartWithSpecificFigure,
                vm => vm.SelectedSpecificStartFigure)
            .Select(_ => Unit.Default);

        IObservable<bool> canExecute =
            obs0.Merge(obs1).Merge(obs2).Merge(obs3)
            .Throttle(TimeSpan.FromMilliseconds(100))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(_ => viewModel)
            .Select(vm => RequiredFiguresAreSelected(vm) && StartWithSpecificFigureIsValid(vm));

        var command = ReactiveCommand
            .Create(DoNothing, canExecute)
            .DisposeWith(disposables);

        command
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .Select(_ => viewModel)
            .SelectMany(async (SearchViewModel vm, CancellationToken ct) => await searchChoreographies.ExecuteAsync(vm, ct))
            .Subscribe(foundChoreographies.Publish)
            .DisposeWith(disposables);

        viewModel.FindChoreography = command;
        return;

        Unit DoNothing() => Unit.Default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool StartWithSpecificFigureIsValid(SearchViewModel vm)
    {
        return !vm.IsStartWithSpecificFigure
               || vm.SelectedSpecificStartFigure is not null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool RequiredFiguresAreSelected(SearchViewModel vm)
        => vm.RequiredFigures.Count(rf => rf.IsSelected) >= 2;
}