using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SliccDB.Serialization;

namespace ChoreoHelper.Database;

public static class DependencyInjection
{
    [UsedImplicitly]
    public static IServiceCollection AddChoreoHelperDatabase(this IServiceCollection services)
    {
        services.AddSingleton<DatabaseConnection>(_ =>
        {
            var databasePath = DatabasePathHelper.GetPathToDatabase();
            return new DatabaseConnection(databasePath);
        });
        return services;
    }
}