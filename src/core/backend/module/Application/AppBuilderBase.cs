namespace Tmobis.Core.Application;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;
using System.Linq;
using System.Reflection;

public abstract class AppBuilderBase<TB, TA>
where TB : IHostApplicationBuilder
where TA : IHost
{
    [Obsolete]
    protected AppBuilderBase(ModuleRegister modules)
    {
        ModulesBuilder = new ModuleRegisterBuilder(
            [.. modules.Select(module => module.GetType())]);
    }

    protected AppBuilderBase(ModuleRegisterBuilder modules)
    {
        ModulesBuilder = modules;
    }

    protected abstract TB Builder { get; }

    protected virtual TA? App { get; set; }

    protected virtual ModuleRegister? Modules { get; set; }

    protected virtual ModuleRegisterBuilder ModulesBuilder { get; }

    public virtual TA Build()
    {
        Register();
        App = BuildApp();
        Configure();
        return App
        ?? throw new InvalidOperationException("Не удалось создать приложение");
    }

    protected abstract TA? BuildApp();

    protected virtual void Register()
    {
        FillModules();
        RegisterModuleRegister();
        RegisterModules();
    }

    protected virtual void FillModules()
    {
        AddCoreModule();
        AddRequiredModules();
    }

    protected virtual void AddCoreModule()
    {
        var coreModule = typeof(CoreModule);
        if (!ModulesBuilder.Contains(coreModule))
        {
            ModulesBuilder.Insert(0, coreModule);
        }
    }

    protected virtual void AddRequiredModules()
    {
        var modulesCopy = ModulesBuilder.ToList();
        ModulesBuilder.Clear();
        foreach (var module in modulesCopy)
        {
            AddModule(module);
        }
    }

    protected virtual void AddModuleRequirements(Type module)
    {
        var deps = module.GetCustomAttribute<ModuleRequirementAttribute>();
        if (deps != null)
        {
            foreach (var depModuleType in deps.Types)
            {
                if (!depModuleType.IsAssignableTo(typeof(IModule)))
                {
                    continue;
                }

                AddModule(depModuleType);
            }
        }
    }

    protected virtual void AddModule(Type module)
    {
        var isNew = !ModulesBuilder.Contains(module);
        if (!isNew)
        {
            return;
        }

        AddModuleRequirements(module);
        var stillNew = !ModulesBuilder.Contains(module);
        if (!stillNew)
        {
            return;
        }
        
        ModulesBuilder.Add(module);
    }

    protected virtual void RegisterModuleRegister()
    {
        Modules = new ModuleRegister(ModulesBuilder);
        Builder.Services.AddSingleton<IModuleRegister>((provider) => Modules);
    }

    protected virtual void RegisterModules()
    {
        if (Modules == null)
        {
            throw new InvalidOperationException(
                "Не удалось получить список модулей");
        }

        foreach (var module in Modules)
        {
            module.PreRegister(
                Builder.Services, Builder.Configuration, Modules);
        }
        foreach (var module in Modules)
        {
            module.Register(
              Builder.Services, Builder.Configuration, Modules);
        }
        foreach (var module in Modules)
        {
            module.PostRegister(
              Builder.Services, Builder.Configuration, Modules);
        }
    }

    protected virtual void Configure()
    {
        if (EqualityComparer<TA>.Default.Equals(App, default))
        {
            throw new InvalidOperationException("Приложение не было построено.");
        }

        ConfigureModules();
    }

    protected virtual void ConfigureModules()
    {
        if (EqualityComparer<TA>.Default.Equals(App, default))
        {
            throw new InvalidOperationException("Приложение не было построено.");
        }
        
        if (Modules == null)
        {
            throw new InvalidOperationException(
                "Не удалось получить список модулей");
        }

        foreach (var module in Modules)
        {
            module.Configure(App.Services, Builder.Configuration, Modules);
        }
    }
}
