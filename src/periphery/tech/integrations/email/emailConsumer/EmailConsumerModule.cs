namespace Tmobis.Integrations.Email.Consumer;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Listeners;
using Tmobis.Core.Modules;
using Tmobis.Core.Medium.Modules;
using Tmobis.Integrations.Email.Consumer.Consumers;
using Tmobis.Core.Medium.Producers;

/// <summary>
/// Модуль бизнес-обработки email-сообщений.
/// </summary>
[ModuleRequirement([typeof(EmailModelModule)])]
public sealed class EmailConsumerModule : ModuleBase, IConsumerModule
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
  }
}
