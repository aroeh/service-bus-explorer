using Azure.Messaging.ServiceBus;
using ServiceBusUtility.Shared.Models;
using System.Text.Json;

namespace ServiceBusUtility.Shared.Mappers;

public static class ServiceBusMapper
{
    public static Message MapToServiceBusMessage(this ServiceBusReceivedMessage receivedMessage)
    {
        return new Message()
        {
            ReceivedMessage = receivedMessage,
            Body = JsonSerializer.SerializeToDocument(receivedMessage.Body.ToString())
        };
    }
}
