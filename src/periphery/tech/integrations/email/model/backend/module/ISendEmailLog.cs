namespace Tmobis.Integrations.Email;

using Tmobis.Core.Models;

public interface ISendEmailLog : IRegisteredEntity
{
    List<string> Addresses { get; set; }
    bool Success { get; set; }

}

