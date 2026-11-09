namespace Tmobis.Integrations.Assistants.Base.Messages;
using Tmobis.Core.Medium.Models;

public record AssistantResponse : StatusMessageResult
{
    public required string? Message { get; init; }
}