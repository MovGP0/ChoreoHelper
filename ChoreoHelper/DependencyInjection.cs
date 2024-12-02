using ChoreoHelper.Algorithms;
using ChoreoHelper.Choreography;
using ChoreoHelper.Dance;
using ChoreoHelper.Distance;
using ChoreoHelper.EditFigure;
using ChoreoHelper.Figure;
using ChoreoHelper.Gateway;
using ChoreoHelper.Graph;
using ChoreoHelper.LevelSelection;
using ChoreoHelper.MainWindow;
using ChoreoHelper.OptionalFigureSelection;
using ChoreoHelper.RequiredFigureSelection;
using ChoreoHelper.Restriction;
using ChoreoHelper.Search;
using ChoreoHelper.SearchResult;
using ChoreoHelper.Transition;
using ChoreoHelper.TransitionEditor;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper;

public static class DependencyInjection
{
    public static IServiceCollection AddChoreoHelper(this IServiceCollection services)
    {
        services.AddMessagePipe();
        services.AddGateway();
        services.AddAlgorithms();
        services.AddChoreoHelperDatabase();

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
        services.AddMainWindow();
        services.AddTransition();
        services.AddTransitionEditor();
    }
}