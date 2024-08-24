using ChoreoHelper.Behaviors.Extensions;
using DynamicData.Binding;

namespace ChoreoHelper.Behaviors.MainWindow;

public sealed class FilterRequiredFiguresBehavior: IBehavior<MainWindowViewModel>
{
    public void Activate(MainWindowViewModel viewModel, CompositeDisposable disposables)
    {
        var requiredFiguresFiltered = new SourceCache<RequiredFigureSelectionViewModel, string>(vm => vm.Hash)
            .DisposeWith(disposables);

        requiredFiguresFiltered
            .Connect()
            .Bind(viewModel.RequiredFiguresFiltered)
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
                 var searchText = vm.SearchText;
                 var figures = viewModel
                     .RequiredFigures
                     .Where(rf => string.IsNullOrEmpty(searchText) || rf.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                     .ToList();

                 requiredFiguresFiltered.Update(figures);
             })
             .DisposeWith(disposables);
    }

    private static IObservable<Unit> Observe(MainWindowViewModel viewModel)
    {
        var listChanged = viewModel.RequiredFigures
            .OnCollectionChanged()
            .Select(_ => Unit.Default);

        var searchTextChanged = viewModel
            .WhenAnyValue(vm => vm.SearchText)
            .Select(_ => Unit.Default);

        return searchTextChanged.Merge(listChanged);
    }
}