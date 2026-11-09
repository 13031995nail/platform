namespace Tmobis.Business.Common.Resources;

using Tmobis.Core.Modules;
using Tmobis.Frameworks.Flexberry.Modules;
using Tmobis.Business.Common.Contacts;

[ModuleRequirement([
    typeof(FlexberryModule), typeof(VerificationRequierementBaseModule), typeof(FlexberryOrganizationTypeBaseModule)
])]
/// <summary>
/// Flexberry бизнес-модуль сущности требований поверки.
/// </summary>
public sealed class FlexberryVerificationRequierementBaseModule : ModuleBase, IEntityModule;
