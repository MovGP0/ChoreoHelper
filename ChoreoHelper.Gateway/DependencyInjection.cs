using Microsoft.Extensions.DependencyInjection;

namespace ChoreoHelper.Gateway;

public static class DependencyInjection
{
    public static IServiceCollection AddGateway(this IServiceCollection services)
    {
        services.AddSingleton<IXmlDataLoader, XmlDataLoader>();
        services.AddSingleton<IXmlDataSaver, XmlDataSaver>();
        return services;
    }
}