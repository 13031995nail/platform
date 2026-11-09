namespace Tmobis.BusinessIntelligence;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;

/// <summary>
/// Модуль бизнес-аналитики.
/// </summary>
public sealed class BusinessIntelligenceModule : ModuleBase, IEntityModule
{
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
  }
}
