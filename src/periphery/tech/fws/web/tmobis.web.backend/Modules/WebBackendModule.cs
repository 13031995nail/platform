namespace Tmobis.Frameworks.Web.Backend.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Business.Core.Modules;
using Tmobis.Business.Core.Providers.Users;
using Tmobis.Business.Core.TownLimit;
using Tmobis.Core.DI;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;
using Tmobis.Frameworks.Web.Backend.Messages;
using Tmobis.Frameworks.Web.Backend.Options;
using Tmobis.Frameworks.Web.Backend.Users;
using Tmobis.Frameworks.Web.Modules;

/// <summary>
/// Модуль backend-инфраструктуры web-приложения.
/// Связывает текущего пользователя и IP с <c>HttpContext</c>, содержит настройки версии и обработку frontend errors.
/// </summary>
[ModuleRequirement([typeof(BusinessCoreModule), typeof(WebFrameworkModule)])]
public class WebBackendModule
: ModuleBase, IConsumerModule
{
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddSingleton<ICurrentUser, CurrentUser>();
    services.AddTransient<IHttpContextIpProvider, HttpContextIpProvider>();
    services.AddOrReplaceScoped<ICurrentUserProvider,
      WebCurrentUserProvider>();
    services.AddOrReplaceScoped<ICurrentTownProvider,
      WebCurrentTownProvider>();
    services.Configure<VersionOptions>(
      configuration.GetSection(VersionOptions.Version));
    services.AddListenedMessage<FrontendError>("WebApi");
  }
}
