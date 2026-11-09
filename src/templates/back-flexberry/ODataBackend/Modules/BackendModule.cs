namespace Tmobis.Backend.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;
using Tmobis.Jobs.Scheduled;

public class BackendModule : ModuleBase, IMessageModule
{
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
    }
}