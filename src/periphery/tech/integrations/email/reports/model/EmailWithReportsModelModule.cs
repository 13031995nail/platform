namespace Tmobis.Integrations.Email.Reports;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;
using Tmobis.Reports;

/// <summary>
/// Модуль контракта отправки email с отчётами.
/// </summary>
[ModuleRequirement([typeof(EmailModelModule), typeof(ReportModelModule)])]
public class EmailWithReportsModelModule : ModuleBase, IEntityModule, IMessageModule
{
    /// <inheritdoc/>
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
    }
}
