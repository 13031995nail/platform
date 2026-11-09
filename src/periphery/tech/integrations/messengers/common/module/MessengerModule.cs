namespace Tmobis.Integrations.Messengers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Business.Core.Modules;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;
using Tmobis.Integrations.Messengers.Providers;

/// <summary>
/// Базовый модуль интеграций с мессенджерами.
/// Содержит message/consumer-контракты и выбор доступного канала связи для сотрудника.
/// </summary>
[ModuleRequirement(
    typeof(BusinessCoreModule),
    typeof(MessengerModelModule))]
public sealed class MessengerModule
    : ModuleBase, IMessageModule, IConsumerModule
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddTransient<
            IEmployeeMessengerProvider,
            EmployeeMessengerProvider>();
    }
}
