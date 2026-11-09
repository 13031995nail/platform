namespace Tmobis.Core.Modules;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ModuleRegister
: ReadOnlyCollection<IModule>,
IModuleRegister
{
    public ModuleRegister(IList<IModule> list) : base(list)
    {

    }

    public ModuleRegister(ModuleRegisterBuilder builder)
    : base([.. builder
        .Select(type => Activator.CreateInstance(type) as IModule
            ?? throw new InvalidOperationException("Не удалось создать модуль"))])
    {

    }
}