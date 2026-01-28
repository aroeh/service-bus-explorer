using Azure.Messaging.ServiceBus;
using ServiceBusUtility.Shared.Models;
using System.Text.Json;

namespace ServiceBusUtility.Shared.Mappers;

public static class ServiceBusMapper
{
    private const string _emptyJsonObject = "{}";
    public static Message MapToServiceBusMessage(this ServiceBusReceivedMessage receivedMessage)
    {
        return new Message()
        {
            ReceivedMessage = receivedMessage,
            Body = MapBodyToJsonDocument(receivedMessage)
        };
    }

    public static JsonDocument MapToJsonDocument(this ServiceBusReceivedMessage receivedMessage)
    {
        Message message = new()
        {
            ReceivedMessage = receivedMessage,
            Body = MapBodyToJsonDocument(receivedMessage)
        };

        string messageJson = JsonSerializer.Serialize(message);
        return messageJson.MapBodyToJsonDocument();
    }

    public static JsonDocument MapBodyToJsonDocument(this ServiceBusReceivedMessage receivedMessage)
    {
        return JsonSerializer.Deserialize<JsonDocument>(receivedMessage.Body.ToString()) ?? JsonDocument.Parse(_emptyJsonObject);
    }

    public static JsonDocument MapBodyToJsonDocument(this string message)
    {
        return JsonSerializer.Deserialize<JsonDocument>(message) ?? JsonDocument.Parse(_emptyJsonObject);
    }
}
