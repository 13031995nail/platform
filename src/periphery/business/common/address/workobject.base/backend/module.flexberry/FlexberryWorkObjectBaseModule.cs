namespace Tmobis.Business.Common.Address;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;
using Tmobis.Frameworks.Flexberry.Modules;

/// <summary>
/// Flexberry бизнес-модуль сущности объектов работ.
/// Определяет правила доступа и ограничения чтения, включая OData.
/// </summary>
[ModuleRequirement([typeof(FlexberryModule), typeof(WorkObjectModuleBase)])]
public sealed class FlexberryWorkObjectBaseModule : ModuleBase, IEntityModule
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddScoped<WorkObjectAccessProvider>();
        services.AddScoped<IWorkObjectAccessProvider>(serviceProvider =>
            serviceProvider.GetRequiredService<WorkObjectAccessProvider>());
        services.AddScoped<IWorkObjectReadLimitProvider>(serviceProvider =>
            serviceProvider.GetRequiredService<WorkObjectAccessProvider>());
        services.AddTransient<Tmobis.Frameworks.Flexberry.Web.OData.Access.IODataReadRestrictionProvider,
            WorkObjectODataReadRestrictionProvider>();
    }
}
