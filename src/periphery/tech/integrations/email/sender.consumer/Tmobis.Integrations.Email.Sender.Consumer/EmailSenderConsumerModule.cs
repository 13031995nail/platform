namespace Tmobis.Integrations.Email.Sender.Consumer;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Medium.Listeners;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;
using Tmobis.Reports;

/// <summary>
/// Модуль бизнес-обработки отправки email.
/// </summary>
[ModuleRequirement([typeof(EmailModelModule)])]
public sealed class EmailSenderConsumerModule : ModuleBase, IConsumerModule
{

}
