using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceBusUtility.Infrastructure.Interfaces;
using ServiceBusUtility.Infrastructure.Models;
using System.Text.Json;

namespace ServiceBusUtility.Infrastructure.Services;

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
    /// <returns><see cref="ServiceBusReceivedMessage"/></returns>
    public async Task<ServiceBusReceivedMessage?> ReceiveMessage()
    {
        _logger.LogInformation("Receiving the next message on the queue");
        ServiceBusReceivedMessage? receivedMessage = await _receiver.ReceiveMessageAsync();

        if (receivedMessage is not null)
        {
            _logger.LogInformation("Completing message and removing from the queue");
            await CompleteMessage(receivedMessage);
        }

        return receivedMessage;
    }

    /// <summary>
    /// Receives and completes messages on the queue
    /// </summary>
    /// <remarks>
    /// Use when the message body is just a string
    /// </remarks>
    /// <param name="maxMessages">Max number of messages to receive</param>
    /// <returns>Collection of <see cref="ServiceBusReceivedMessage"/></returns>
    public async Task<IReadOnlyList<ServiceBusReceivedMessage>> ReceiveMessages(int maxMessages)
    {
        _logger.LogInformation("Receiving the next {count} messages on the queue", maxMessages);
        IReadOnlyList<ServiceBusReceivedMessage> receivedMessages = await _receiver.ReceiveMessagesAsync(maxMessages);

        _logger.LogInformation("Received {count} messages", receivedMessages.Count);
        if (receivedMessages.Count > 0)
        {
            await CompleteMessages(receivedMessages);
        }

        return receivedMessages;
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
    /// <returns><see cref="ServiceBusReceivedMessage"/></returns>
    public async Task<ServiceBusReceivedMessage?> PeekMessage(long? sequence = null)
    {
        _logger.LogInformation("Peeking message on the queue");
        ServiceBusReceivedMessage? receivedMessage = await _receiver.PeekMessageAsync(sequence);

        return receivedMessage;
    }

    /// <summary>
    /// Views messages on the queue without removing them
    /// </summary>
    /// <remarks>
    /// Use when the message body is just a string
    /// </remarks>
    /// <param name="maxMessages">Max number of messages to view</param>
    /// <param name="startSequence">Start sequence to view messages</param>
    /// <returns>Collection of <see cref="ServiceBusReceivedMessage"/></returns>
    public async Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekMessages(int maxMessages, long startSequence)
    {
        _logger.LogInformation("Peeking messages on the queue");
        IReadOnlyList<ServiceBusReceivedMessage> receivedMessages = await _receiver.PeekMessagesAsync(maxMessages, startSequence);

        _logger.LogInformation("Peeked at {count} messages", receivedMessages.Count);

        return receivedMessages;
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
}
