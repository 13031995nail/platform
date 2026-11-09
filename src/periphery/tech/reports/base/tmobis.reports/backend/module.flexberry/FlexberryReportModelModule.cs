namespace Tmobis.Reports.Flexberry;

using Tmobis.Core.Modules;
using Tmobis.Frameworks.Flexberry.Modules;

/// <summary>
/// Модуль контрактов и сущностей подсистемы отчетов и печатных форм на основе Flexberry.
/// </summary>
[ModuleRequirement([typeof(FlexberryModule), typeof(ReportModelModule)])]
public sealed class FlexberryReportModelModule
: ModuleBase, IEntityModule;