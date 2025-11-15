using Azure.Messaging.ServiceBus;

namespace ServiceBusUtility.Infrastructure.Interfaces;

public interface IQueueApi
{
    /// <summary>
    /// Publishes a message to the service bus
    /// </summary>
    /// <remarks>
    /// Serializes the payload into a string before publishing
    /// </remarks>
    /// <typeparam name="T">Type of the payload</typeparam>
    /// <param name="payload">Message payload</param>
    /// <returns>Awaitable task</returns>
    Task Publish<T>(T payload);

    /// <summary>
    /// Receives and completes a message on the queue
    /// </summary>
    /// <remarks>
    /// Use when the message body is just a string
    /// </remarks>
    /// <returns><see cref="ServiceBusReceivedMessage"/></returns>
    Task<ServiceBusReceivedMessage?> ReceiveMessage();

    /// <summary>
    /// Receives and completes messages on the queue
    /// </summary>
    /// <remarks>
    /// Use when the message body is just a string
    /// </remarks>
    /// <param name="maxMessages">Max number of messages to receive</param>
    /// <returns>Collection of <see cref="ServiceBusReceivedMessage"/></returns>
    Task<IReadOnlyList<ServiceBusReceivedMessage>> ReceiveMessages(int maxMessages);

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
    Task<ServiceBusReceivedMessage?> PeekMessage(long? sequence = null);

    /// <summary>
    /// Views messages on the queue without removing them
    /// </summary>
    /// <remarks>
    /// Use when the message body is just a string
    /// </remarks>
    /// <param name="maxMessages">Max number of messages to view</param>
    /// <param name="startSequence">Start sequence to view messages</param>
    /// <returns>Collection of <see cref="ServiceBusReceivedMessage"/></returns>
    Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekMessages(int maxMessages, long startSequence);
}
