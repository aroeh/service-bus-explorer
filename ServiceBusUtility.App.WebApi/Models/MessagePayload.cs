namespace ServiceBusUtility.App.WebApi.Models;

public record MessagePayload
{
    public string Payload { get; set; } = default!;
    public string[]? Tags { get; set; } = [];
}
