namespace Tmobis.Business.Process.Documents.Flexberry;

using System.Reflection;
using Tmobis.Business.Process.Documents.Base.Flexberry;
using Tmobis.Core.Modules;

/// <summary>
/// Flexberry бизнес-модуль сущности актов.
/// </summary>
[ModuleRequirement([typeof(BaseActBaseFlexberryModule)])]
public sealed class BaseActFlexberryModule : ModuleBase, IEntityModule;