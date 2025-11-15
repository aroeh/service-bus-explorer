using Microsoft.Extensions.DependencyInjection;
using ServiceBusUtility.Infrastructure.Interfaces;
using ServiceBusUtility.Infrastructure.Services;

namespace ServiceBusUtility.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceBusApi(this IServiceCollection services)
    {
        services.AddScoped<IQueueApi, QueueApi>();

        return services;
    }
}
