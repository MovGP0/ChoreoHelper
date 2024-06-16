﻿using System.Diagnostics;
using System.Globalization;
using System.Reactive.Disposables;
using ChoreoHelper.Database;
using DynamicData.Binding;
using ReactiveUI;
using Splat;

namespace ChoreoHelper.ViewModels;

[DebuggerDisplay("{DebuggerDisplay}")]
public sealed class ChoreographyViewModel : ReactiveObject, IDisposable
{
    private CompositeDisposable Disposables { get; } = new();

    private float _rating;
    public float Rating
    {
        get => _rating;
        set => this.RaiseAndSetIfChanged(ref _rating, value);
    }

    public IObservableCollection<DanceStepNodeInfo> Figures { get; }
        = new ObservableCollectionExtended<DanceStepNodeInfo>();

    public ChoreographyViewModel()
    {
        if (this.IsInDesignMode())
        {
            Rating = 5;
            for (var i = 0; i < 5; i++)
            {
                Figures.Add(new DanceStepNodeInfo("Foobar", "01234", DanceLevel.Gold));
            }
        }

        foreach (var behavior in Locator.Current.GetServices<IBehavior<ChoreographyViewModel>>())
        {
            behavior.Activate(this, Disposables);
        }
    }

    private bool _isDisposed;

    public void Dispose() => Dispose(true);

    ~ChoreographyViewModel() => Dispose(false);

    private void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        Disposables.Dispose();
        
        if (disposing)
        {
            GC.SuppressFinalize(this);
        }

        _isDisposed = true;
    }

    private string DebuggerDisplay
    {
        get
        {
            var rating = Rating.ToString(CultureInfo.InvariantCulture);
            var figures = string.Join(", ", Figures.Select(f => f.Name));
            return $"Rating = {rating}; Figures = [{figures}]";
        }
    }
}