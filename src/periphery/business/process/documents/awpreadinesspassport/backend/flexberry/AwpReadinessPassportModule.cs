namespace Tmobis.Business.Process.Documents;

using Tmobis.Frameworks.Flexberry.Web.OData.Access;

using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Business.Common.Address;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;

/// <summary>
/// Flexberry бизнес-модуль сущности паспортов готовности ОЗП.
/// Определяет проверку доступа и ограничения чтения через OData.
/// </summary>
public sealed class AwpReadinessPassportModule : ModuleBase, IEntityModule, IConsumerModule
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddScoped<AwpReadinessPassportAccess>();
        services.AddTransient<IODataReadRestrictionProvider,
            AwpReadinessPassportReadRestrictionProvider>();
    }
}
