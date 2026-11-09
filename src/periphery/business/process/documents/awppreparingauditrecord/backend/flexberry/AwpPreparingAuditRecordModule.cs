namespace Tmobis.Business.Process.Documents;

using Tmobis.Frameworks.Flexberry.Web.OData.Access;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Process.Documents.Providers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Modules;
using Tmobis.Reports.Models;

/// <summary>
/// Flexberry бизнес-модуль сущности журнала контроля подготовки ОЗП.
/// Содержит providers рабочего пространства, документов, замечаний и требований, а также обработку подписания связанных документов.
/// </summary>
public sealed class AwpPreparingAuditRecordModule : ModuleBase, IEntityModule
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddTransient<IMessageConsumer<AwpReadinessPassportSigned, IEntityUpdatedResult>,
            OnAwpReadinessPassportSigned>();
        services.AddTransient<IMessageConsumer<ReadinessActSigned, IEntityUpdatedResult>,
            OnReadinessActSigned>();
        services.AddTransient<IMessageConsumer<EvaluationSheetSigned, IEntityUpdatedResult>,
            OnEvaluationSheetSigned>();
        services.AddTransient<IAwpPreparingAuditRecordProvider, AwpPreparingAuditRecordProvider>();
        services.AddTransient<IWorkspaceDashboardProvider, WorkspaceDashboardProvider>();
        services.AddTransient<WorkspaceObjectScopeProvider>();
        services.AddTransient<IWorkObjectProcessProvider, WorkObjectProcessProvider>();
        services.AddTransient<WorkObjectDocumentsProvider>();
        services.AddTransient<WorkObjectRemarksProvider>();
        services.AddTransient<WorkObjectRequirementMapper>();
        services.AddScoped<AwpPreparingAuditRecordAccess>();
        services.AddScoped<AwpPreparingAuditRecordCommissionAccess>();
        services.AddScoped<ActSigningCancellationService>();
        foreach (var registration in WorkObjectDocumentReportRegistrations.BuiltIn)
        {
            services.AddSingleton(registration);
        }
        services.AddTransient<IReportAuthorizationProvider, WorkObjectDocumentReportAuthorization>();
        services.AddTransient<IODataReadRestrictionProvider,
            AwpPreparingAuditRecordReadRestrictionProvider>();
    }
}
