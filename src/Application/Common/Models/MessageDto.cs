namespace Template_net10.Application.Common.Models;

/// <summary>Payload for message-only operations (update / delete) that return no entity data.</summary>
public sealed class MessageDto
{
    public required string Message { get; init; }

    public static MessageDto Of(string message) => new() { Message = message };
}
