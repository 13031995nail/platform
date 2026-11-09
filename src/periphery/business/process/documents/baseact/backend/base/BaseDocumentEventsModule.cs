namespace Tmobis.Business.Process.Documents.Base;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Modules;

/// <summary>
/// Бизнес-модуль базовой обработки событий актов.
/// Обрабатывает создание базового акта.
/// </summary>
public sealed class BaseDocumentEventsModule : ModuleBase
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddTransient<
            IMessageConsumer<EntityCreated<IBaseActBase>, IEntityUpdatedResult>,
            OnBaseActCreatedBase>();
    }
}
