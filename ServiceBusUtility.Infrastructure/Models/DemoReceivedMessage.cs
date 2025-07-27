using Azure.Messaging.ServiceBus;

namespace ServiceBusUtility.Infrastructure.Models;

public class DemoReceivedMessage
{
    public ServiceBusReceivedMessage? ServiceBusReceivedMessage { get; set; }

    public object? Body { get; set; }
}
