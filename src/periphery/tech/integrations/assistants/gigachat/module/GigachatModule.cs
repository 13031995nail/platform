namespace Tmobis.Integrations.Assistants.Gigachat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Modules;

/// <summary>
/// Модуль интеграции с GigaChat.
/// Содержит настройки, adapter и message-контракты интеграции.
/// </summary>
public class GigachatModule : ModuleBase, IMessageModule
{
    /// <inheritdoc/>
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.Configure<GigachatOptions>(
            configuration.GetSection(GigachatOptions.Gigachat));
        services.AddTransient<GigachatAdapter>();
    }
}
