using ServiceBusUtility.Infrastructure.Models;

namespace ServiceBusUtility.Infrastructure.Interfaces;

public interface IQueueApi
{
    /// <summary>
    /// Publishes a message to the service bus
    /// </summary>
    /// <remarks>
    /// Publishes a string
    /// </remarks>
    /// <param name="payload">Message payload</param>
    /// <returns>Awaitable task</returns>
    Task Publish(string payload);

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
    /// <returns><see cref="DemoReceivedMessage"/></returns>
    Task<DemoReceivedMessage?> ReceiveMessage();

    /// <summary>
    /// Receives and completes a message on the queue
    /// </summary>
    /// <remarks>
    /// Use when the message body needs to be deserialized into a type
    /// </remarks>
    /// <typeparam name="T">Type of the payload</typeparam>
    /// <returns><see cref="DemoReceivedMessage"/></returns>
    Task<DemoReceivedMessage?> ReceiveMessage<T>();

    /// <summary>
    /// Receives and completes messages on the queue
    /// </summary>
    /// <remarks>
    /// Use when the message body is just a string
    /// </remarks>
    /// <param name="maxMessages">Max number of messages to view</param>
    /// <returns>Collection of <see cref="DemoReceivedMessage"/></returns>
    Task<IReadOnlyList<DemoReceivedMessage>> ReceiveMessages(int maxMessages);

    /// <summary>
    /// Receives and completes messages on the queue
    /// </summary>
    /// <remarks>
    /// Use when the message body needs to be deserialized into a type
    /// </remarks>
    /// <typeparam name="T">Type of the message body</typeparam>
    /// <param name="maxMessages">Max number of messages to view</param>
    /// <returns>Collection of <see cref="DemoReceivedMessage"/></returns>
    Task<IReadOnlyList<DemoReceivedMessage>> ReceiveMessages<T>(int maxMessages);

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
    Task<DemoReceivedMessage?> PeekMessage(long? sequence = null);

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
    Task<DemoReceivedMessage?> PeekMessage<T>(long? sequence = null);

    /// <summary>
    /// Views messages on the queue without removing them
    /// </summary>
    /// <remarks>
    /// Use when the message body is just a string
    /// </remarks>
    /// <param name="maxMessages">Max number of messages to view</param>
    /// <param name="startSequence">Start sequence to view messages</param>
    /// <returns>Collection of <see cref="DemoReceivedMessage"/></returns>
    Task<IReadOnlyList<DemoReceivedMessage>> PeekMessages(int maxMessages, long startSequence);

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
    Task<IReadOnlyList<DemoReceivedMessage>> PeekMessages<T>(int maxMessages, long startSequence);
}
