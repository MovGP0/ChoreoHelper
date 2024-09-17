using System.Collections.Immutable;
using ChoreoHelper.Behaviors.Extensions;
using ChoreoHelper.Messages;
using DynamicData.Binding;

namespace ChoreoHelper.Behaviors.Search;

public sealed class FilterOptionalFiguresBehavior: IBehavior<SearchViewModel>
{
    public void Activate(SearchViewModel viewModel, CompositeDisposable disposables)
    {
        var optionalFiguresFiltered = new SourceCache<OptionalFigureSelectionViewModel, string>(vm => vm.Hash)
            .DisposeWith(disposables);

        optionalFiguresFiltered
            .Connect()
            .Bind(viewModel.OptionalFiguresFiltered)
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Subscribe()
            .DisposeWith(disposables);

        Observe(viewModel)
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Select(_ => viewModel)
            .Subscribe(vm =>
            {
                var selectedRequiredFigureHashes = vm.RequiredFiguresFiltered
                    .Where(rf => rf.IsSelected)
                    .Select(rf => rf.Hash)
                    .ToImmutableHashSet();

                var searchText = vm.SearchText;
                var figures = viewModel
                    .OptionalFigures
                    .Where(rf => string.IsNullOrEmpty(searchText) || rf.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                    .Where(of => !selectedRequiredFigureHashes.Contains(of.Hash))
                    .ToImmutableArray();

                optionalFiguresFiltered.Update(figures);
            })
            .DisposeWith(disposables);
    }

    private static IObservable<Unit> Observe(SearchViewModel viewModel)
    {
        var lastCount = 0;
        var listChanged = viewModel.OptionalFigures
            .WhenPropertyChanged(x => x.Count)
            .Where(_ => viewModel.OptionalFigures.Count != lastCount)
            .Do(_ => lastCount = viewModel.OptionalFigures.Count)
            .Select(_ => Unit.Default);

        var searchTextChanged = viewModel
            .WhenAnyValue(
                vm => vm.SearchText,
                vm => vm.SelectedDance)
            .Select(_ => Unit.Default);

        var requiredFigureChanged = MessageBus.Current.Listen<RequiredFigureUpdated>()
            .Select(_ => Unit.Default);

        return searchTextChanged
            .Merge(listChanged)
            .Merge(requiredFigureChanged);
    }
}