namespace Tmobis.Integrations.Email.Sender.MailKit;

using global::MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Medium.Producers;
using Tmobis.Integrations.Email;


public class MailKitSender(IOptions<MailKitOptions> options) : IEmailSender
{
  private readonly MailKitOptions options = options.Value;

  public Task Publish(IMessage message)
  {
    throw new NotImplementedException();
  }

  /// <inheritdoc/>
  public Task SendEmail(SendEmailPrepared msg)
  {
    var message = new MimeMessage();
    
    InternetAddressList list = new InternetAddressList();
    foreach (var adress in msg.Addresses)
    {
      list.Add(new MailboxAddress("", adress));
    }

    var builder = new BodyBuilder();
    builder.TextBody = msg.Body;
    foreach (var file in msg.Files)
    {
      builder.Attachments.Add(file.Key, file.Value);
    }

    message.From.Add(new MailboxAddress(options.FromName, options.FromAddress));
    message.Body = builder.ToMessageBody();
    message.To.AddRange(list);
    message.Subject = msg.Subject;

    using (var client = new SmtpClient())
    {
      client.Connect(options.Server, options.Port, options.UseSSL);
      if (!string.IsNullOrEmpty(options.Login) && !string.IsNullOrEmpty(options.Password))
      {
        client.Authenticate(options.Login, options.Password);
      }
      client.Send(message);
      client.Disconnect(true);
    }
    return Task.CompletedTask;
  }

  public Task<IMessageResult> SendWithResult(IMessage message)
  {
    throw new NotImplementedException();
  }
}
