using ChoreoHelper.Behaviors.Extensions;
using DynamicData.Binding;

namespace ChoreoHelper.Behaviors.MainWindow;

public sealed class FilterRequiredFiguresBehavior: IBehavior<MainWindowViewModel>
{
    public void Activate(MainWindowViewModel viewModel, CompositeDisposable disposables)
    {
        var requiredFiguresFiltered = new SourceCache<RequiredFigureSelectionViewModel, string>(vm => vm.Hash);

        requiredFiguresFiltered
            .Connect()
            .Bind(viewModel.RequiredFiguresFiltered)
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Subscribe()
            .DisposeWith(disposables);

        var lastCount = 0;
        var listChanged = viewModel.RequiredFigures
            .WhenPropertyChanged(x => x.Count)
            .Where(_ => viewModel.OptionalFigures.Count != lastCount)
            .Do(_ => lastCount = viewModel.OptionalFigures.Count)
            .Select(_ => Unit.Default);

        var searchTextChanged =viewModel
            .WhenAnyValue(
                vm => vm.SearchText,
                vm => vm.SelectedDance)
            .Select(_ => Unit.Default);

         searchTextChanged.Merge(listChanged)
             .ObserveOn(RxApp.MainThreadScheduler)
             .SubscribeOn(RxApp.MainThreadScheduler)
             .Select(_ => viewModel)
             .Subscribe(vm =>
             {
                 var searchText = vm.SearchText;
                 var figures = viewModel
                     .RequiredFigures
                     .Where(rf => string.IsNullOrEmpty(searchText) || rf.Name.Contains(searchText))
                     .ToList();

                 requiredFiguresFiltered.Update(figures);
             })
             .DisposeWith(disposables);
    }
}