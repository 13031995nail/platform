namespace Tmobis.PrimaryAdapters.BaseWcfService.Modules;

using CoreWCF.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tmobis.Core.Modules;

/// <summary>
/// Модуль первичного адаптера WCF на основе CoreWCF.
/// </summary>
public class BaseWcfServiceModule : ModuleBase
{
  /// <inheritdoc/>
  public override void PreRegister(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    services.AddServiceModelServices();
    services.AddServiceModelMetadata();
  }
}
