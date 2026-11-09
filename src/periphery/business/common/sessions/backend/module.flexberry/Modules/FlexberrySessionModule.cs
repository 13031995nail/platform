namespace Tmobis.Business.Common.Sessions.Flexberry.Modules;

using Tmobis.Core.Modules;
using Tmobis.Frameworks.Flexberry.Modules;
using Tmobis.Business.Common.Sessions.Modules;

/// <summary>
/// Flexberry бизнес-модуль сущности пользовательских сессий.
/// </summary>
[ModuleRequirement([typeof(FlexberryModule), typeof(UserSessionsModule)])]
public sealed class FlexberrySessionModule : ModuleBase, IEntityModule;