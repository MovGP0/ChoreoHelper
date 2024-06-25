using System.Text;
using System.Windows;
using DynamicData.Binding;

namespace ChoreoHelper.Behaviors.Choreography;

public sealed class CopyBehavior : IBehavior<ChoreographyViewModel>
{
    public void Activate(ChoreographyViewModel viewModel, CompositeDisposable disposables)
    {
        IObservable<bool> canExecute = viewModel.Figures
            .WhenAnyPropertyChanged()
            .Select(figures => figures is { Count: > 0 });

        var command = ReactiveCommand
            .Create(DoNothing, canExecute)
            .DisposeWith(disposables);

        disposables.Add(Disposable.Create(() => viewModel.Copy = DisabledCommand.Instance));
        viewModel.Copy = command;

        command
            .ObserveOn(RxApp.MainThreadScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Select(_ => viewModel)
            .Subscribe(vm =>
            {
                var sb = new StringBuilder();

                foreach(var figure in vm.Figures)
                {
                    sb.AppendLine($"{figure.Name} ({figure.Level})");
                }

                Clipboard.Clear();
                Clipboard.SetText(sb.ToString());
            })
            .DisposeWith(disposables);

        return;

        Unit DoNothing() => Unit.Default;
    }
}