using ChoreoHelper.Choreography;
using ChoreoHelper.Entities;
using DynamicData.Binding;
using JetBrains.Annotations;
using ReactiveUI.Extensions;

namespace ChoreoHelper.SearchResult;

public sealed class SearchResultViewModel: ReactiveObject, IActivatableViewModel, IDisposable, IRoutableViewModel
{
    public IObservableCollection<ChoreographyViewModel> Choreographies { get; }
        = new ObservableCollectionExtended<ChoreographyViewModel>();

    public SearchResultViewModel()
    {
        HostScreen = null!;
        if (this.IsInDesignMode())
        {
            InitializeDesignModeData();
        }

        this.WhenActivated(this.ActivateBehaviors);
    }

    [UsedImplicitly]
    public SearchResultViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;
        if (this.IsInDesignMode())
        {
            InitializeDesignModeData();
        }

        this.WhenActivated(this.ActivateBehaviors);
    }

    private void InitializeDesignModeData()
    {
        for (var i = 0; i < 5; i++)
        {
            Choreographies.Add(new ChoreographyViewModel()
            {
                Rating = i,
                Figures = { new(), new(), new(), new() }
            });
        }
    }

    public ViewModelActivator Activator { get; } = new();

    public void Dispose() => Activator.Dispose();

    public string UrlPathSegment => "search-result";
    public IScreen HostScreen { get; }
}