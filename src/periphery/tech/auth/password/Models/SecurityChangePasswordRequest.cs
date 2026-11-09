namespace Tmobis.Auth.Password.Models;

using Tmobis.Core.Medium.Models;

[MessageMetadata(Group = "security")]
public record SecurityChangePasswordRequest : DomainCommand<SecurityChangePasswordResponse>
{
    public Guid EmployeeId { get; set; }

    public string NewPwd { get; set; }

    public string OldPwd { get; set; }
}

public record SecurityChangePasswordResponse : IMessageResult
{
    public bool Success { get; set; } = true;

    public string Error { get; set; }

    public SecurityChangePasswordResponse() { }

    public SecurityChangePasswordResponse(string error)
    {
        Success = false;
        Error = error;
    }
}
