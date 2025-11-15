using Microsoft.Extensions.DependencyInjection;
using ServiceBusUtility.Core.Interfaces;
using ServiceBusUtility.Core.Orchestrators;

namespace ServiceBusUtility.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQueueExplorer(this IServiceCollection services)
    {
        services.AddScoped<IQueueExplorer, QueueExplorer>();

        return services;
    }
}

