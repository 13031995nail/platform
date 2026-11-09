namespace Tmobis.Business.Process.Documents;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;

/// <summary>
/// Бизнес-модуль построения номеров актов.
/// </summary>
public sealed class ActNumberBuilderModule : ModuleBase
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddTransient<IActNumberBuilder, ActNumberBuilder>();
    }
}
