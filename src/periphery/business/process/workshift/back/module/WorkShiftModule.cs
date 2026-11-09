namespace Tmobis.Business.Process.WorkShift;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Business.Process.WorkShift.Messages;
using Tmobis.Core.DI;
using Tmobis.Core.Entities;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;

/// <summary>
/// Бизнес-модуль сущности рабочих смен.
/// Содержит сообщения и операции получения текущей смены, запуска, завершения и передачи задач.
/// </summary>
public sealed class WorkShiftModule : ModuleBase, IEntityModule, IConsumerModule, IMessageModule
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddTransient<IEntitySeeder, WorkShiftEntitySeeder>();
        services.AddListenedMessage<GetTodayWorkShiftId>("WebApi");
        services.AddListenedMessage<StartWorkShiftTask>("WebApi");
        services.AddListenedMessage<FinishWorkShiftTask>("WebApi");
        services.AddListenedMessage<TransferWorkShiftTask>("WebApi");
    }
}