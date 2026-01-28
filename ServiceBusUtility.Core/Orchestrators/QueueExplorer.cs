using Azure.Messaging.ServiceBus;
using ServiceBusUtility.Core.Interfaces;
using ServiceBusUtility.Infrastructure.Interfaces;
using ServiceBusUtility.Shared.Mappers;
using ServiceBusUtility.Shared.Models;
using System.Text.Json;

namespace ServiceBusUtility.Core.Orchestrators;

public class QueueExplorer(IQueueApi queueApi) : IQueueExplorer
{
    private readonly IQueueApi _queueApi = queueApi;

    /// <summary>
    /// Publishes a message to the service bus
    /// </summary>
    /// <param name="payload">Message payload</param>
    public async Task Publish<T>(T payload)
    {
        await _queueApi.Publish(payload);
    }

    /// <summary>
    /// Receives and completes a message on the queue
    /// </summary>
    /// <returns><see cref="Message"/></returns>
    public async Task<Message?> ReceiveMessage()
    {
        var receivedMessage = await _queueApi.ReceiveMessage();
        return receivedMessage?.MapToServiceBusMessage();
    }

    /// <summary>
    /// Receives and completes messages on the queue
    /// </summary>
    /// <param name="maxMessages">Max number of messages to receive</param>
    /// <returns>Collection of <see cref="Message"/></returns>
    public async Task<IReadOnlyList<Message>> ReceiveMessages(int maxMessages)
    {
        var receivedMessages = await _queueApi.ReceiveMessages(maxMessages);
        return [.. receivedMessages.Select(_ => _.MapToServiceBusMessage())];
    }

    /// <summary>
    /// Views a message on the queue without removing
    /// </summary>
    /// <param name="includeMetaData">Indicate if message metadata should be returned</param>
    /// <param name="sequence">Message sequence to view</param>
    /// <returns><see cref="JsonDocument"/></returns>
    public async Task<JsonDocument?> PeekMessage(bool includeMetaData, long? sequence = null)
    {
        var receivedMessage = await _queueApi.PeekMessage(sequence);
        
        if (receivedMessage is null)
        {
            return null;
        }

        return includeMetaData
            ? receivedMessage?.MapToJsonDocument()
            : receivedMessage?.MapBodyToJsonDocument();
    }

    /// <summary>
    /// Views messages on the queue without removing them
    /// </summary>
    /// <param name="maxMessages">Max number of messages to view</param>
    /// <param name="startSequence">Start sequence to view messages</param>
    /// <param name="includeMetaData">Indicate if message metadata should be returned</param>
    /// <returns>Collection of <see cref="JsonDocument"/></returns>
    public async Task<IReadOnlyList<JsonDocument>> PeekMessages(int maxMessages, long startSequence, bool includeMetaData)
    {
        IReadOnlyList<ServiceBusReceivedMessage> receivedMessages = await _queueApi.PeekMessages(maxMessages, startSequence);

        if (receivedMessages is null || receivedMessages.Count == 0)
        {
            return [];
        }

        return includeMetaData
            ? [.. receivedMessages.Select(_ => _.MapToJsonDocument())]
            : [.. receivedMessages.Select(_ => _.MapBodyToJsonDocument())];
    }
}

