namespace Tmobis.Business.Process.Documents.Flexberry;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;
using Tmobis.Business.Core.Models.Files;
using Tmobis.Core.DI;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Modules;

/// <summary>
/// Flexberry бизнес-модуль сущности подписей документов.
/// Определяет проверку доступа к подписям и обработку удаления связанных файлов.
/// </summary>
public sealed class ActSignFlexberryModule : ModuleBase, IEntityModule
{
    public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddScoped<ActSignAccessValidator>();
        services.AddOrReplaceTransient<IMessageConsumer<EntityDeleted<IFile>, IEntityUpdatedResult>, OnFileDeletedConsumer>();
    }
}
