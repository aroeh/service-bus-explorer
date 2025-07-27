using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceBusUtility.Infrastructure.Interfaces;
using ServiceBusUtility.Infrastructure.Models;
using System.Text.Json;

namespace ServiceBusUtility.Infrastructure.Services;

/// <summary>
/// This class demonstrates working with strings and complex objects as the message content body.
/// It is likely more normal to work with complex objects since this will help provide structure
/// and consistency around the messages
/// </summary>
public class QueueApi : IQueueApi
{
    private readonly ILogger<QueueApi> _logger;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusReceiver _receiver;
    private readonly ServiceBusSender _sender;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public QueueApi(IConfiguration config, ILogger<QueueApi> logger)
    {
        _logger = logger;

        var serviceBusConfig = config.GetRequiredSection("ServiceBusConfig").Get<ServiceBusConfig>();

        ValidateConfiguration(serviceBusConfig);

        _client = new(serviceBusConfig?.ConnectionString);
        _receiver = _client.CreateReceiver(serviceBusConfig?.QueueName);
        _sender = _client.CreateSender(serviceBusConfig?.QueueName);
    }

    private void ValidateConfiguration(ServiceBusConfig? serviceBusConfig)
    {
        if (serviceBusConfig is null)
        {
            const string error = "Service Bus Configuration was not found.  Unable to use the Service Bus APIs";
            _logger.LogError(error);
            throw new Exception(error);
        }

        if(string.IsNullOrWhiteSpace(serviceBusConfig.Namespace) && string.IsNullOrWhiteSpace(serviceBusConfig.ConnectionString))
        {
            const string error = "Service Bus Configuration requires Namespace or ConnectionString";
            _logger.LogError(error);
            throw new Exception(error);
        }
    }

    /// <summary>
    /// Publishes a message to the service bus
    /// </summary>
    /// <remarks>
    /// Publishes a string
    /// </remarks>
    /// <param name="payload">Message payload</param>
    /// <returns>Awaitable task</returns>
    public async Task Publish(string payload)
    {
        _logger.LogInformation("Publishing string payload: {payload}", payload);
        await _sender.SendMessageAsync(new ServiceBusMessage(payload));
    }

    /// <summary>
    /// Publishes a message to the service bus
    /// </summary>
    /// <remarks>
    /// Serializes the payload into a string before publishing
    /// </remarks>
    /// <typeparam name="T">Type of the payload</typeparam>
    /// <param name="payload">Message payload</param>
    /// <returns>Awaitable task</returns>
    public async Task Publish<T>(T payload)
    {
        _logger.LogInformation("Publishing typed payload: {payload}", payload);
        string json = JsonSerializer.Serialize(payload, _serializerOptions);
        await _sender.SendMessageAsync(new ServiceBusMessage(json));
    }

    /// <summary>
    /// Receives and completes a message on the queue
    /// </summary>
    /// <remarks>
    /// Use when the message body is just a string
    /// </remarks>
    /// <returns><see cref="DemoReceivedMessage"/></returns>
    public async Task<DemoReceivedMessage?> ReceiveMessage()
    {
        _logger.LogInformation("Receiving the next message on the queue");
        ServiceBusReceivedMessage? receivedMessage = await _receiver.ReceiveMessageAsync();

        if (receivedMessage is not null)
        {
            _logger.LogInformation("Completing message and removing from the queue");
            await CompleteMessage(receivedMessage);
        }

        return ParseReceivedMessage(receivedMessage);
    }

    /// <summary>
    /// Receives and completes a message on the queue
    /// </summary>
    /// <remarks>
    /// Use when the message body needs to be deserialized into a type
    /// </remarks>
    /// <typeparam name="T">Type of the payload</typeparam>
    /// <returns><see cref="DemoReceivedMessage"/></returns>
    public async Task<DemoReceivedMessage?> ReceiveMessage<T>()
    {
        _logger.LogInformation("Receiving the next message on the queue");
        ServiceBusReceivedMessage? receivedMessage = await _receiver.ReceiveMessageAsync();

        if (receivedMessage is not null)
        {
            _logger.LogInformation("Completing message and removing from the queue");
            await CompleteMessage(receivedMessage);
        }

        return ParseReceivedMessage<T>(receivedMessage);
    }

    /// <summary>
    /// Receives and completes messages on the queue
    /// </summary>
    /// <remarks>
    /// Use when the message body is just a string
    /// </remarks>
    /// <param name="maxMessages">Max number of messages to view</param>
    /// <returns>Collection of <see cref="DemoReceivedMessage"/></returns>
    public async Task<IReadOnlyList<DemoReceivedMessage>> ReceiveMessages(int maxMessages)
    {
        _logger.LogInformation("Receiving the next {count} messages on the queue", maxMessages);
        IReadOnlyList<ServiceBusReceivedMessage> receivedMessages = await _receiver.ReceiveMessagesAsync(maxMessages);

        _logger.LogInformation("Received {count} messages", receivedMessages.Count);
        if (receivedMessages.Count > 0)
        {
            await CompleteMessages(receivedMessages);
        }

        return ParseReceivedMessages(receivedMessages);
    }

    /// <summary>
    /// Receives and completes messages on the queue
    /// </summary>
    /// <remarks>
    /// Use when the message body needs to be deserialized into a type
    /// </remarks>
    /// <typeparam name="T">Type of the message body</typeparam>
    /// <param name="maxMessages">Max number of messages to view</param>
    /// <returns>Collection of <see cref="DemoReceivedMessage"/></returns>
    public async Task<IReadOnlyList<DemoReceivedMessage>> ReceiveMessages<T>(int maxMessages)
    {
        _logger.LogInformation("Receiving the next {count} messages on the queue", maxMessages);
        IReadOnlyList<ServiceBusReceivedMessage> receivedMessages = await _receiver.ReceiveMessagesAsync(maxMessages);

        _logger.LogInformation("Received {count} messages", receivedMessages.Count);
        if (receivedMessages.Count > 0)
        {
            await CompleteMessages(receivedMessages);
        }

        return ParseReceivedMessages<T>(receivedMessages);
    }

    /// <summary>
    /// Views a message on the queue without removing
    /// </summary>
    /// <remarks>
    /// <para>Use when the message body is just a string</para>
    /// <para>
    /// <paramref name="sequence"/> is optional.  
    /// If not provided peek will view the message at the latest sequence
    /// </para>
    /// </remarks>
    /// <param name="sequence">Message sequence to view</param>
    /// <returns><see cref="DemoReceivedMessage"/></returns>
    public async Task<DemoReceivedMessage?> PeekMessage(long? sequence = null)
    {
        _logger.LogInformation("Peeking message on the queue");
        ServiceBusReceivedMessage? receivedMessage = await _receiver.PeekMessageAsync(sequence);

        return ParseReceivedMessage(receivedMessage);
    }

    /// <summary>
    /// Views a message on the queue without removing
    /// </summary>
    /// <remarks>
    /// <para>Use when the message body needs to be deserialized into a type</para>
    /// <para>
    /// <paramref name="sequence"/> is optional.  
    /// If not provided peek will view the message at the latest sequence
    /// </para>
    /// </remarks>
    /// <typeparam name="T">Type of the payload</typeparam>
    /// <param name="sequence">Message sequence to view</param>
    /// <returns><see cref="DemoReceivedMessage"/></returns>
    public async Task<DemoReceivedMessage?> PeekMessage<T>(long? sequence = null)
    {
        _logger.LogInformation("Peeking message on the queue");
        ServiceBusReceivedMessage? receivedMessage = await _receiver.PeekMessageAsync(sequence);

        return ParseReceivedMessage<T>(receivedMessage);
    }

    /// <summary>
    /// Views messages on the queue without removing them
    /// </summary>
    /// <remarks>
    /// Use when the message body is just a string
    /// </remarks>
    /// <param name="maxMessages">Max number of messages to view</param>
    /// <param name="startSequence">Start sequence to view messages</param>
    /// <returns>Collection of <see cref="DemoReceivedMessage"/></returns>
    public async Task<IReadOnlyList<DemoReceivedMessage>> PeekMessages(int maxMessages, long startSequence)
    {
        _logger.LogInformation("Peeking messages on the queue");
        IReadOnlyList<ServiceBusReceivedMessage> receivedMessages = await _receiver.PeekMessagesAsync(maxMessages, startSequence);

        _logger.LogInformation("Peeked at {count} messages", receivedMessages.Count);

        return ParseReceivedMessages(receivedMessages);
    }

    /// <summary>
    /// Views messages on the queue without removing them
    /// </summary>
    /// <remarks>
    /// Use when the message body needs to be deserialized into a type
    /// </remarks>
    /// <typeparam name="T">Type of the message body</typeparam>
    /// <param name="maxMessages">Max number of messages to view</param>
    /// <param name="startSequence">Start sequence to view messages</param>
    /// <returns>Collection of <see cref="DemoReceivedMessage"/></returns>
    public async Task<IReadOnlyList<DemoReceivedMessage>> PeekMessages<T>(int maxMessages, long startSequence)
    {
        _logger.LogInformation("Peeking messages on the queue");
        IReadOnlyList<ServiceBusReceivedMessage> receivedMessages = await _receiver.PeekMessagesAsync(maxMessages, startSequence);

        _logger.LogInformation("Peeked at {count} messages", receivedMessages.Count);

        return ParseReceivedMessages<T>(receivedMessages);
    }

    private async Task CompleteMessages(IReadOnlyList<ServiceBusReceivedMessage> receivedMessages)
    {
        foreach (var msg in receivedMessages)
        {
            await CompleteMessage(msg);
        }
    }

    private async Task CompleteMessage(ServiceBusReceivedMessage receivedMessage)
    {
        await _receiver.CompleteMessageAsync(receivedMessage);
    }

    private List<DemoReceivedMessage> ParseReceivedMessages(IReadOnlyList<ServiceBusReceivedMessage> receivedMessages)
    {
        if(receivedMessages.Count == 0)
        {
            return [];
        }

        List<DemoReceivedMessage> messages = [];
        foreach(var msg in receivedMessages)
        {
            var parsed = ParseReceivedMessage(msg);
            if (parsed is not null)
            {
                messages.Add(parsed);
            }
        }
        return messages;
    }

    private DemoReceivedMessage? ParseReceivedMessage(ServiceBusReceivedMessage? receivedMessage)
    {
        if(receivedMessage is null)
        {
            _logger.LogInformation("No message was found in the queue");
            return null;
        }

        // Read the service bus message body as a string
        string body = receivedMessage.Body.ToString();

        return new()
        {
            ServiceBusReceivedMessage = receivedMessage,
            Body = body
        };
    }

    private List<DemoReceivedMessage> ParseReceivedMessages<T>(IReadOnlyList<ServiceBusReceivedMessage> receivedMessages)
    {
        if (receivedMessages.Count == 0)
        {
            return [];
        }

        List<DemoReceivedMessage> messages = [];
        foreach (var msg in receivedMessages)
        {
            var parsed = ParseReceivedMessage<T>(msg);
            if(parsed is not null)
            {
                messages.Add(parsed);
            }
        }
        return messages;
    }

    private DemoReceivedMessage? ParseReceivedMessage<T>(ServiceBusReceivedMessage? receivedMessage)
    {
        if (receivedMessage is null)
        {
            _logger.LogInformation("No message was found in the queue");
            return null;
        }

        // Read the service bus message body as a string
        string body = receivedMessage.Body.ToString();
        var deserialized = JsonSerializer.Deserialize<T>(body, _serializerOptions);

        return new()
        {
            ServiceBusReceivedMessage = receivedMessage,
            Body = deserialized
        };
    }
}
