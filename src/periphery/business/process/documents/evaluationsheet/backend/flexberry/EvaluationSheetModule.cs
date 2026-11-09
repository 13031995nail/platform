namespace Tmobis.Business.Process.Documents;

using Tmobis.Frameworks.Flexberry.Web.OData.Access;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;
using Tmobis.Business.Common.Address;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;

/// <summary>
/// Flexberry бизнес-модуль сущности оценочных листов.
/// Содержит сервисы работы и проверки листов, выборку, полномочия комиссии и ограничения чтения через OData.
/// </summary>
public sealed class EvaluationSheetModule : ModuleBase, IEntityModule, IConsumerModule
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddTransient<EvaluationSheetService>();
        services.AddTransient<EvaluationSheetReviewService>();
        services.AddScoped<EvaluationSheetPermissionProvider>();
        services.AddScoped<IWorkObjectAdditionalReadLimitProvider, EvaluationSheetCommissionReadLimitProvider>();
        services.AddTransient<EvaluationSheetSelectionProvider>();
        services.AddScoped<EvaluationSheetAccess>();
        services.AddTransient<IODataReadRestrictionProvider,
            EvaluationSheetReadRestrictionProvider>();
    }
}
