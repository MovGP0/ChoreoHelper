using System.Diagnostics;
using OneOf;
using OneOf.Types;

namespace ChoreoHelper.ViewModels;

[DebuggerDisplay("{Description}")]
public sealed class DistanceViewModel : ReactiveObject
{
    [Reactive] public OneOf<float, None> Distance { get; set; } = new None();
    [Reactive] public string Description { get; set; } = string.Empty;
}