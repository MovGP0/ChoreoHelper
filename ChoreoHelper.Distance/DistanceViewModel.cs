using System.Diagnostics;
using ReactiveUI.Extensions;

namespace ChoreoHelper.Distance;

[DebuggerDisplay("{Description}")]
public sealed partial class DistanceViewModel : ReactiveObject
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

    [Reactive] private Entities.Distance _distance = new Unknown();
    [Reactive] private string _description = string.Empty;
}