namespace Tmobis.Core.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Базовый шаблон модуля.
/// </summary>
public abstract class ModuleBase : IModule
{
  /// <inheritdoc/>
  public virtual void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister moduleRegister)
  {
  }

  /// <inheritdoc/>
  public virtual void PreRegister(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister moduleRegister)
  {
  }

  /// <inheritdoc/>
  public virtual void PostRegister(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister moduleRegister)
  {
  }

  /// <inheritdoc/>
  public virtual void Configure(
    IServiceProvider services,
    IConfiguration configuration,
    IModuleRegister moduleRegister)
  {
  }

  /// <inheritdoc/>
  public virtual void PreConfigure(
    IServiceProvider services,
    IConfiguration configuration,
    IModuleRegister moduleRegister)
  {
  }

  /// <inheritdoc/>
  public virtual void PostConfigure(
    IServiceProvider services,
    IConfiguration configuration,
    IModuleRegister moduleRegister)
  {
  }
}
