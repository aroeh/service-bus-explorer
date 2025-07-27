using Microsoft.AspNetCore.Mvc;
using ServiceBusUtility.App.WebApi.Models;
using ServiceBusUtility.Infrastructure.Interfaces;
using ServiceBusUtility.Infrastructure.Models;
using System.Net;

namespace ServiceBusUtility.App.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ServiceBusController
(
    ILogger<ServiceBusController> logger,
    IQueueApi queueApi
) : ControllerBase
{
    private readonly ILogger<ServiceBusController> _logger = logger;
    private readonly IQueueApi _queueApi = queueApi;

    /// <summary>
    /// Publish a new message to the queue
    /// </summary>
    /// <param name="payload">Message payload</param>
    /// <returns></returns>
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [HttpPost]
    public async Task<IResult> Publish([FromBody] MessagePayload payload)
    {
        _logger.LogInformation("Publishing new message");
        await _queueApi.Publish(payload.Payload);
        return TypedResults.Ok("Message successfully published");
    }

    /// <summary>
    /// Receive the next message on the queue
    /// </summary>
    /// <returns>Collection of <see cref="DemoReceivedMessage"/></returns>
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [HttpGet("receive")]
    public async Task<IResult> Receive()
    {
        _logger.LogInformation("Receiving next messages");
        DemoReceivedMessage? messages = await _queueApi.ReceiveMessage();
        return TypedResults.Ok(messages);
    }

    /// <summary>
    /// Receives and completes messages on the queue
    /// </summary>
    /// <param name="max">Max number of messages to view</param>
    /// <returns>Collection of <see cref="DemoReceivedMessage"/></returns>
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [HttpGet("receive-messages")]
    public async Task<IResult> ReceiveMessages([FromQuery] int max)
    {
        _logger.LogInformation("Receiving messages");
        IReadOnlyList<DemoReceivedMessage> messages = await _queueApi.ReceiveMessages(max);
        return TypedResults.Ok(messages);
    }

    /// <summary>
    /// View a message on the queue
    /// </summary>
    /// <param name="sequence">Message sequence to view</param>
    /// <returns><see cref="DemoReceivedMessage"/></returns>
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [HttpGet("peek")]
    public async Task<IResult> PeekMessage([FromQuery] long start)
    {
        _logger.LogInformation("Peeking a message");
        DemoReceivedMessage? message = await _queueApi.PeekMessage(start);
        return TypedResults.Ok(message);
    }

    /// <summary>
    /// View messages on the queue
    /// </summary>
    /// <param name="max">Max number of messages to view</param>
    /// <param name="start">Message sequence to start from when viewing</param>
    /// <returns>Collection of <see cref="DemoReceivedMessage"/></returns>
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [HttpGet("peek-messages")]
    public async Task<IResult> PeekMessages([FromQuery] int max, [FromQuery] long start)
    {
        _logger.LogInformation("Peeking at messages");
        IReadOnlyList<DemoReceivedMessage> messages = await _queueApi.PeekMessages(max, start);
        return TypedResults.Ok(messages);
    }

    /// <summary>
    /// Publish a new typed message to the queue
    /// </summary>
    /// <param name="payload">Message payload</param>
    /// <returns></returns>
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [HttpPost("typed")]
    public async Task<IResult> PublishTyped([FromBody] MessagePayload payload)
    {
        _logger.LogInformation("Publishing new typed message");
        await _queueApi.Publish<MessagePayload>(payload);
        return TypedResults.Ok("Message successfully published");
    }

    /// <summary>
    /// Receive the next message on the queue
    /// </summary>
    /// <returns>Collection of <see cref="DemoReceivedMessage"/></returns>
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [HttpGet("receive-typed")]
    public async Task<IResult> ReceiveTyped()
    {
        _logger.LogInformation("Receiving next messages");
        DemoReceivedMessage? messages = await _queueApi.ReceiveMessage<MessagePayload>();
        return TypedResults.Ok(messages);
    }

    /// <summary>
    /// Receives and completes messages on the queue
    /// </summary>
    /// <param name="max">Max number of messages to view</param>
    /// <returns>Collection of <see cref="DemoReceivedMessage"/></returns>
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [HttpGet("receive-typed-messages")]
    public async Task<IResult> ReceiveTypedMessages([FromQuery] int max)
    {
        _logger.LogInformation("Receiving messages");
        IReadOnlyList<DemoReceivedMessage> messages = await _queueApi.ReceiveMessages<MessagePayload>(max);
        return TypedResults.Ok(messages);
    }

    /// <summary>
    /// View a typed message on the queue
    /// </summary>
    /// <param name="sequence">Message sequence to view</param>
    /// <returns><see cref="DemoReceivedMessage"/></returns>
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [HttpGet("peek-typed")]
    public async Task<IResult> PeekTypedMessage([FromQuery] long start)
    {
        _logger.LogInformation("Peeking a message");
        DemoReceivedMessage? message = await _queueApi.PeekMessage<MessagePayload>(start);
        return TypedResults.Ok(message);
    }

    /// <summary>
    /// View typed messages on the queue
    /// </summary>
    /// <param name="max">Max number of messages to view</param>
    /// <param name="start">Message sequence to start from when viewing</param>
    /// <returns>Collection of <see cref="DemoReceivedMessage"/></returns>
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [HttpGet("peek-typed-messages")]
    public async Task<IResult> PeekTypedMessages([FromQuery] int max, [FromQuery] long start)
    {
        _logger.LogInformation("Peeking at messages");
        IReadOnlyList<DemoReceivedMessage> messages = await _queueApi.PeekMessages<MessagePayload>(max, start);
        return TypedResults.Ok(messages);
    }
}
