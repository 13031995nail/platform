namespace Tmobis.Flexberry.Autoincrement.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Business.Core.Flexberry.Modules;
using Tmobis.Core.Modules;
using Tmobis.Flexberry.Autoincrement.LockingAutoincrementService;
using Tmobis.Flexberry.Autoincrement.LockingService;
using Tmobis.Flexberry.Autoincrement.Objects;
using Tmobis.Frameworks.Flexberry.Modules;

/// <summary>
/// Модуль автоинкремента Flexberry.
/// Содержит хранилище счётчиков, locking service и генерацию последовательных значений.
/// </summary>
[ModuleRequirement([typeof(FlexberryModule), typeof(FlexberryBusinessCoreModule)])]
public class AutoincrementModule: ModuleBase
{
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddTransient<ICounterStorage, DBCounterStorage>();
    services.AddTransient<ILockingService, DefaultLockingService>();
    services.AddTransient<IAutoincrementService, LockingAutoincrementService>();
  }
}
