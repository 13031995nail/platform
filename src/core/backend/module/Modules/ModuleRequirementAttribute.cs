namespace Tmobis.Core.Modules;

[AttributeUsage(AttributeTargets.Class)]
public class ModuleRequirementAttribute(params Type[] types)
: Attribute
{
    public Type[] Types { get; private set; } = types;
}