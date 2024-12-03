using System.Diagnostics;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Distance;

[DebuggerDisplay("{Description}")]
public sealed class DistanceViewModel : ReactiveObject
{
    public DistanceViewModel()
    {
        if (this.IsInDesignMode())
        {
            InitializeDesignModeData();
        }
    }

    private void InitializeDesignModeData()
    {
        Distance = 1f;
        Description = "Some Description";
    }

    [Reactive] public Entities.Distance Distance { get; set; } = new Unknown();
    [Reactive] public string Description { get; set; } = string.Empty;
}