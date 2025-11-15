using ServiceBusUtility.Core.Interfaces;
using ServiceBusUtility.Infrastructure.Interfaces;
using ServiceBusUtility.Shared.Mappers;
using ServiceBusUtility.Shared.Models;

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
    /// <param name="sequence">Message sequence to view</param>
    /// <returns><see cref="Message"/></returns>
    public async Task<Message?> PeekMessage(long? sequence = null)
    {
        var receivedMessage = await _queueApi.PeekMessage(sequence);
        Type? classType = Type.GetType("MessagePayload");
        //var test = receivedMessage?.MapToServiceBusMessage<classType> ();
        return receivedMessage?.MapToServiceBusMessage();
    }

    /// <summary>
    /// Views messages on the queue without removing them
    /// </summary>
    /// <param name="maxMessages">Max number of messages to view</param>
    /// <param name="startSequence">Start sequence to view messages</param>
    /// <returns>Collection of <see cref="Message"/></returns>
    public async Task<IReadOnlyList<Message>> PeekMessages(int maxMessages, long startSequence)
    {
        var receivedMessages = await _queueApi.PeekMessages(maxMessages, startSequence);
        return [.. receivedMessages.Select(_ => _.MapToServiceBusMessage())];
    }
}

