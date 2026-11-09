namespace Tmobis.Integrations.Email.Service.Modules;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.DI;
using Tmobis.Core.Medium.Listeners;
using Tmobis.Core.Medium.Mappers;
using Tmobis.Core.Medium.Options;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Modules;
using Tmobis.Integrations.Email.Sender.MailKit;

/// <summary>
/// Модуль сервисного WebApi-сценария отправки email.
/// Предоставляет сообщение <c>SendEmailPrepared</c> через transport <c>WebApi</c>.
/// </summary>
public sealed class EmailServiceModule : ModuleBase
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddListenedMessage<SendEmailPrepared>("WebApi");
  }

}
