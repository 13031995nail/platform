namespace Tmobis.Auth.Password.Models;

using Tmobis.Core.Medium.Models;

[MessageMetadata(Group = "security")]
public record SetIsComplexPasswordRequest : DomainCommand<SetIsComplexPasswordResponse>
{
    public Guid UserId { get; set; }

    public string Pwd { get; set; }
}

public record SetIsComplexPasswordResponse : IMessageResult
{
    public bool Success { get; set; } = true;

    public string? Error { get; set; }

    public SetIsComplexPasswordResponse() { }

    public SetIsComplexPasswordResponse(string error)
    {
        Success = false;
        Error = error;
    }
}
