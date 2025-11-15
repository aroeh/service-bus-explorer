using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace ServiceBusUtility.Shared.Models;

public class Message
{
    public ServiceBusReceivedMessage? ReceivedMessage { get; set; }
    public JsonDocument? Body { get; set; }
}
