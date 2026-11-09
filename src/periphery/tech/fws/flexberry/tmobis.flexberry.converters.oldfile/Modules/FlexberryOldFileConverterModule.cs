namespace Tmobis.Frameworks.Flexberry.Converters.OldFile.Modules;
using Tmobis.Core.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Frameworks.Flexberry.Converters.OldFile.Jobs;
using Tmobis.Frameworks.Flexberry.Converters.OldFile.Converters;

/// <summary>
/// Модуль конвертации legacy WebFile-данных Flexberry.
/// Содержит converter и background job переноса старых файлов.
/// </summary>
public sealed class FlexberryOldFileConverterModule : ModuleBase
{
    /// <inheritdoc/>
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddTransient<IOldWebFileConverter, OldWebFileConverter>();
        services.AddHostedService<OldWebFileJob>();
    }
}
