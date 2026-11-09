namespace Tmobis.Utils.Lzw;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;
using Tmobis.Utils.Lzw.Converters;

/// <summary>
/// Модуль LZW сжатия данных.
/// </summary>
public class LzwModule : ModuleBase
{
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        base.Register(services, configuration, modules);
        services.AddTransient<ILzwConverter, LzwConverter>();
    }
}
