namespace Tmobis.Core.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public interface IModule
{
  /// <summary>
  /// Регистрация сервисов в DI .Net (в первую очередь).
  /// </summary>
  /// <param name="services">Коллекция сервисов.</param>
  /// <param name="configuration">Конфигурация приложения.</param>
  public void PreRegister(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister moduleRegister);

  /// <summary>
  /// Регистрация сервисов в DI .Net (в последнюю очередь).
  /// </summary>
  /// <param name="services">Коллекция сервисов.</param>
  /// <param name="configuration">Конфигурация приложения.</param>
  public void PostRegister(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister moduleRegister);

  /// <summary>
  /// Регистрация сервисов в DI .Net.
  /// </summary>
  /// <param name="services">Коллекция сервисов.</param>
  /// <param name="configuration">Конфигурация приложения.</param>
  /// <param name="moduleRegister">Список модулей.</param>
  public void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister moduleRegister);

  /// <summary>
  /// Настройка инстанции приложения (в первую очередь).
  /// </summary>
  /// <param name="app">Приложение.</param>
  /// <param name="configuration">Конфигурация приложения.</param>
  public void PreConfigure(
    IServiceProvider services,
    IConfiguration configuration,
    IModuleRegister moduleRegister);

  /// <summary>
  /// Настройка инстанции приложения (в последнюю очередь).
  /// </summary>
  /// <param name="app">Приложение.</param>
  /// <param name="configuration">Конфигурация приложения.</param>
  public void PostConfigure(
    IServiceProvider services,
    IConfiguration configuration,
    IModuleRegister moduleRegister);

  /// <summary>
  /// Настройка инстанции приложения.
  /// </summary>
  /// <param name="services">Коллекция сервисов.</param>
  /// <param name="configuration">Конфигурация приложения.</param>
  /// <param name="moduleRegister">Список модулей.</param>
  public void Configure(
    IServiceProvider services,
    IConfiguration configuration,
    IModuleRegister moduleRegister);
}
