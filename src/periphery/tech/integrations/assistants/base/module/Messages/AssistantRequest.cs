namespace Tmobis.Integrations.Assistants.Base.Messages;
using Tmobis.Core.Medium.Models;

public record AssistantRequest : Message<AssistantResponse>
{
    public required string Message { get; init; }

    public string? FileName { get; init; }

    public string? FileMime { get; init; }

    public bool HasFile { get; init; } = false;

    public byte[]? File { get; init; }
}