namespace Tmobis.Backend.Objects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;

public class ObjectsModule : ModuleBase, IEntityModule
{
    /// <inheritdoc/>
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        RegisterEntities(services);
    }

    private void RegisterEntities(IServiceCollection services)
    {
    }

    /// <inheritdoc/>
}
