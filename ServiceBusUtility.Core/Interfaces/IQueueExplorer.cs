using ServiceBusUtility.Shared.Models;

namespace ServiceBusUtility.Core.Interfaces;

public interface IQueueExplorer
{
    /// <summary>
    /// Publishes a message to the service bus
    /// </summary>
    /// <param name="payload">Message payload</param>
    Task Publish<T>(T payload);

    /// <summary>
    /// Receives and completes a message on the queue
    /// </summary>
    /// <returns><see cref="Message"/></returns>
    Task<Message?> ReceiveMessage();

    /// <summary>
    /// Receives and completes messages on the queue
    /// </summary>
    /// <param name="maxMessages">Max number of messages to receive</param>
    /// <returns>Collection of <see cref="Message"/></returns>
    Task<IReadOnlyList<Message>> ReceiveMessages(int maxMessages);

    /// <summary>
    /// Views a message on the queue without removing
    /// </summary>
    /// <param name="sequence">Message sequence to view</param>
    /// <returns><see cref="Message"/></returns>
    Task<Message?> PeekMessage(long? sequence = null);

    /// <summary>
    /// Views messages on the queue without removing them
    /// </summary>
    /// <param name="maxMessages">Max number of messages to view</param>
    /// <param name="startSequence">Start sequence to view messages</param>
    /// <returns>Collection of <see cref="Message"/></returns>
    Task<IReadOnlyList<Message>> PeekMessages(int maxMessages, long startSequence);
}

