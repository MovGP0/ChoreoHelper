using System.Collections.Immutable;

namespace ReactiveUI.TestObjects;

/// <summary>
/// Represents a list of properties that have changed in a given batch.
/// </summary>
public sealed class BatchPropertyChangedEventArgs : EventArgs
{
    public BatchPropertyChangedEventArgs(ImmutableArray<string> changedProperties)
    {
        ChangedProperties = changedProperties;
    }

    /// <summary>
    /// The list of property names that have changed.
    /// </summary>
    public ImmutableArray<string> ChangedProperties { get; }
}