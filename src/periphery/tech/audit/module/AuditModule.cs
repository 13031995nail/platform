namespace Tmobis.Audit;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;

/// <summary>
/// Базовый модуль аудита.
/// Содержит контракт записи аудита, реализацию по умолчанию и настройки подсистемы.
/// </summary>
public class AuditModule : ModuleBase
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister moduleRegister)
    {
        base.Register(services, configuration, moduleRegister);
        services.AddTransient<IAuditWriter, EmptyAuditWriter>();
        services.AddTransient(sp => new Lazy<IAuditWriter>(sp.GetRequiredService<IAuditWriter>));
        services.Configure<AuditOptions>(
            configuration.GetSection(AuditOptions.Audit));
    }
}
