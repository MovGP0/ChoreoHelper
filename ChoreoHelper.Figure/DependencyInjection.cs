using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.Figure;

public static class DependencyInjection
{
    public static IServiceCollection AddFigure(this IServiceCollection services)
    {
        services.AddTransient<FigureViewModel>();
        return services;
    }
}