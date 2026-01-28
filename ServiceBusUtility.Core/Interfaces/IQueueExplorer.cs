using ServiceBusUtility.Shared.Models;
using System.Text.Json;

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
    /// <param name="includeMetaData">Indicate if message metadata should be returned</param>
    /// <param name="sequence">Message sequence to view</param>
    /// <returns><see cref="JsonDocument"/></returns>
    Task<JsonDocument?> PeekMessage(bool includeMetaData, long? sequence = null);

    /// <summary>
    /// Views messages on the queue without removing them
    /// </summary>
    /// <param name="maxMessages">Max number of messages to view</param>
    /// <param name="startSequence">Start sequence to view messages</param>
    /// <param name="includeMetaData">Indicate if message metadata should be returned</param>
    /// <returns>Collection of <see cref="JsonDocument"/></returns>
    Task<IReadOnlyList<JsonDocument>> PeekMessages(int maxMessages, long startSequence, bool includeMetaData);
}

