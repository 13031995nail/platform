namespace Tmobis.Objects.EF;

using Business.Core.Models.Employees;
using Business.Override1;
using Business.Override2;
using Core.DI;
using Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Modules;
using Tmobis.Integrations.Messengers;

/// <summary>
/// Модуль объектной модели приложения.
/// </summary>
[ModuleRequirement(
    typeof(EntityOverrideTest1Module),
    typeof(EntityOverrideTest2Module),
    typeof(MessengerModelModule))]
public class TestEFObjectsModule : ModuleBase, IEntityModule
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister moduleRegister)
    {
        base.Register(services, configuration, moduleRegister);
        services.AddOrReplaceKeyedTransient<
            IEmployee, Tmobis.Backend.EF.Employee>(
            nameof(IBusinessEntity));
        services.AddOrReplaceKeyedTransient<
            IEmployee, Tmobis.Backend.EF.Employee>(
            nameof(IStoredEntity));
    }
}
