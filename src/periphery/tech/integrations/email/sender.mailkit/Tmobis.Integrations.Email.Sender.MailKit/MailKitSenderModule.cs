namespace Tmobis.Integrations.Email.Sender.MailKit;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Modules;

/// <summary>
/// Модуль отправки email через MailKit.
/// Содержит реализацию <c>IEmailSender</c> и настройки транспорта.
/// </summary>
[ModuleRequirement([typeof(EmailModelModule)])]
public class MailKitSenderModule : ModuleBase
{
  /// <inheritdoc/>
  public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddTransient<IEmailSender, MailKitSender>();
    services.Configure<MailKitOptions>(
      configuration.GetSection(MailKitOptions.MailKitSender));
  }
}
