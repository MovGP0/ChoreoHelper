using ChoreoHelper.Messages;

namespace ChoreoHelper.Behaviors.Algorithms;

public interface ISearchChoreographies
{
    Task<FoundChoreographies> ExecuteAsync(SearchViewModel viewModel, CancellationToken ct);
}