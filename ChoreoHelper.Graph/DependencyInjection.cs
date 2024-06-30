using ChoreoHelper.Gateway;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.Graph;

public static class DependencyInjection
{
    [UsedImplicitly]
    public static IServiceCollection AddChoreoHelperDatabase(this IServiceCollection services)
    {
        services.AddSingleton<IDanceFiguresRepository, DanceFiguresRepository>();
        return services;
    }
}