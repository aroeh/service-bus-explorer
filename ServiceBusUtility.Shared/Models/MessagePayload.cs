namespace ServiceBusUtility.Shared.Models;

public record MessagePayload
{
    public string Text { get; set; } = default!;
    public string[]? Tags { get; set; } = [];
}
