using ChoreoHelper.Algorithms;
using ChoreoHelper.Choreography;
using ChoreoHelper.Dance;
using ChoreoHelper.Distance;
using ChoreoHelper.EditFigure;
using ChoreoHelper.Entities;
using ChoreoHelper.Figure;
using ChoreoHelper.Gateway;
using ChoreoHelper.LevelSelection;
using ChoreoHelper.OptionalFigureSelection;
using ChoreoHelper.RequiredFigureSelection;
using ChoreoHelper.Restriction;
using ChoreoHelper.Search;
using ChoreoHelper.SearchResult;
using ChoreoHelper.Shell;
using ChoreoHelper.Transition;
using ChoreoHelper.TransitionEditor;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper;

public static class DependencyInjection
{
    public static IServiceCollection AddChoreoHelper(this IServiceCollection services)
    {
        services.AddSingleton<DancesCache>();
        services.AddMessagePipe();
        services.AddGateway();
        services.AddAlgorithms();
        AddControls(services);
        return services;
    }

    private static void AddControls(IServiceCollection services)
    {
        services.AddChoreography();
        services.AddDance();
        services.AddDistance();
        services.AddEditFigure();
        services.AddFigure();
        services.AddLevelSelection();
        services.AddOptionalFigureSelection();
        services.AddRequiredFigureSelection();
        services.AddRestriction();
        services.AddSearch();
        services.AddSearchResult();
        services.AddShell();
        services.AddTransition();
        services.AddTransitionEditor();
    }
}