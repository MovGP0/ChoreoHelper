using System.Diagnostics;

namespace ChoreoHelper.Distance;

[DebuggerDisplay("{Description}")]
public sealed class DistanceViewModel : ReactiveObject
{
    [Reactive] public Entities.Distance Distance { get; set; } = new Unknown();
    [Reactive] public string Description { get; set; } = string.Empty;
}