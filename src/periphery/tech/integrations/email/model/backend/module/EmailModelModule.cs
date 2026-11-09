namespace Tmobis.Integrations.Email;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;

/// <summary>
/// Модуль модели и контрактов email-интеграции.
/// </summary>
public class EmailModelModule : ModuleBase, IEntityModule, IMessageModule
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
