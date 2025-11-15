using Microsoft.AspNetCore.Mvc;
using ServiceBusUtility.Core.Interfaces;
using ServiceBusUtility.Shared.Models;
using System.Net;

namespace ServiceBusUtility.App.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ServiceBusController
(
    ILogger<ServiceBusController> logger,
    IQueueExplorer queueExplorer
) : ControllerBase
{
    private readonly ILogger<ServiceBusController> _logger = logger;
    private readonly IQueueExplorer _queueExplorer = queueExplorer;

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
        await _queueExplorer.Publish(payload);
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
        var messages = await _queueExplorer.ReceiveMessage();
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
        var messages = await _queueExplorer.ReceiveMessages(max);
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
        var message = await _queueExplorer.PeekMessage(start);
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
        var messages = await _queueExplorer.PeekMessages(max, start);
        return TypedResults.Ok(messages);
    }
}
