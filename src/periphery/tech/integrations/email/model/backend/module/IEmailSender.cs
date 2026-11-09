namespace Tmobis.Integrations.Email;

public interface IEmailSender
{
    Task SendEmail(SendEmailPrepared request);
}
