using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.EditFigure;

public static class DependencyInjection
{
    public static IServiceCollection AddEditFigure(this IServiceCollection services)
    {
        services.AddTransient<EditFigureViewModel>();
        services.AddTransient<IViewFor<EditFigureViewModel>, EditFigureView>();
        return services;
    }
}