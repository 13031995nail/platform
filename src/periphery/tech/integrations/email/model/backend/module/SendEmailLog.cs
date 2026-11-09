namespace Tmobis.Integrations.Email;

using Tmobis.Core.Models;

public class SendEmailLog : BusinessEntity, ISendEmailLog
{
    public List<string> Addresses { get; set; }
    public bool Success { get; set; }
}
