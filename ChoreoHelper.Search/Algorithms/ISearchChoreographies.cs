using ChoreoHelper.Messages;

namespace ChoreoHelper.Search.Algorithms;

public interface ISearchChoreographies
{
    Task<FoundChoreographies> ExecuteAsync(SearchViewModel viewModel, CancellationToken ct);
}