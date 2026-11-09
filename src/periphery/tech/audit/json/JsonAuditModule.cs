namespace Tmobis.Audit.Json;

using Core.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;

/// <summary>
/// Модуль JSON-аудита.
/// Использует <c>JsonAuditWriter</c> для записи аудита и содержит связанные сущности.
/// </summary>
[ModuleRequirement(typeof(AuditModule))]
public class JsonAuditModule : ModuleBase, IEntityModule
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister moduleRegister)
    {
        base.Register(services, configuration, moduleRegister);
        services.AddOrReplaceTransient<IAuditWriter, JsonAuditWriter>();
    }
}
