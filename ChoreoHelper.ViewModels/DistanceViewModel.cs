using System.Diagnostics;
using OneOf;
using OneOf.Types;

namespace ChoreoHelper.ViewModels;

[DebuggerDisplay("{Description}")]
public sealed class DistanceViewModel : ReactiveObject
{
    [Reactive] public Distance Distance { get; set; } = new Unknown();
    [Reactive] public string Description { get; set; } = string.Empty;
}