namespace Tmobis.Business.Common.Personnel;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;

/// <summary>
/// Бизнес-модуль сущности приёмных комиссий.
/// Содержит сервисы работы с комиссиями и их сотрудниками.
/// </summary>
public sealed class AdmissionCommitteeModule : ModuleBase, IEntityModule, IConsumerModule
{
    public override void Register(
       IServiceCollection services,
       IConfiguration configuration,
       IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddTransient<AdmissionCommitteeMembershipProvider>();
        services.AddTransient<AdmissionCommitteeService>();
        services.AddTransient<AdmissionCommitteeEmployeeService>();
        services.AddScoped<AdmissionCommitteeAccess>();
    }
}
