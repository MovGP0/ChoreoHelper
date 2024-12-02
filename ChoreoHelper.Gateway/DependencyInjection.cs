using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.Gateway;

public static class DependencyInjection
{
    public static IServiceCollection AddGateway(this IServiceCollection services)
    {
        services.AddSingleton<XmlDataLoader>();
        services.AddSingleton<XmlDataSaver>();
        return services;
    }
}