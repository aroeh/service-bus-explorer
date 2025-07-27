using Azure.Messaging.ServiceBus;

namespace ServiceBusUtility.Infrastructure.Models;

public class ServiceBusConfig
{
    public string? Namespace { get; set; } = default!;

    // "Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;"
    public string? ConnectionString { get; set; } = default!;

    public string QueueName { get; set; } = default!;

    public ServiceBusTransportType TransportType { get; set; } = ServiceBusTransportType.AmqpTcp;
}
