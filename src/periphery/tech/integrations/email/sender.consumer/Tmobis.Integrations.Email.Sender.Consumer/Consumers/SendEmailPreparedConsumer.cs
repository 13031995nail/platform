namespace Tmobis.Integrations.Email.Sender.Consumer;

using Core.Medium.Consumers;
using Core.Repositories;
using Email;

public class SendEmailPreparedConsumer(
    IEmailSender emailSender,
    IRepository repository
) : IMessageConsumer<SendEmailPrepared, SendEmailResult>
{
    public async Task<SendEmailResult> ConsumeWithResult(SendEmailPrepared message)
    {
        var success = false;
        try
        {
            await emailSender.SendEmail(message);
            success = true;
            return new SendEmailResult { Error = false };
        }
        catch (Exception ex)
        {
            success = false;
            return new SendEmailResult { Error = true };
        }
        finally
        {
            var log = repository.MakeEntity<ISendEmailLog>();
            log.Success = success;
            log.Addresses = message.Addresses;
            log.Creator = "SendEmailPreparedConsumer";
            log.CreateTime = DateTime.UtcNow;
            repository.Create(log);
        }
    }
}
