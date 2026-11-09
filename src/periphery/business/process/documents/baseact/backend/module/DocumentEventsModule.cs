namespace Tmobis.Business.Process.Documents;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Business.Process.Documents.Base;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Modules;

/// <summary>
/// Бизнес-модуль обработки событий актов.
/// Обрабатывает создание базового акта.
/// </summary>
public sealed class DocumentEventsModule : ModuleBase
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddTransient<
            IMessageConsumer<EntityCreated<IBaseActBase>, IEntityUpdatedResult>,
            OnBaseActCreated>();
    }
}
