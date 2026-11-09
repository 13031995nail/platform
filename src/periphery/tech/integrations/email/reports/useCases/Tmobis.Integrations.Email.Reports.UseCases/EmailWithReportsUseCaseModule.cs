namespace Tmobis.Integrations.Email.Reports.UseCases;

using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;
using Tmobis.Integrations.Email.Consumer;
using Tmobis.Reports.Stimulsoft.Client.Adapter;

/// <summary>
/// Модуль сценариев формирования отчётов и их отправки по email.
/// </summary>
[ModuleRequirement([typeof(EmailWithReportsModelModule), typeof(EmailConsumerModule), typeof(StimulsoftAdapterModule)])]
public class EmailWithReportsUseCaseModule : ModuleBase, IConsumerModule
{

}
