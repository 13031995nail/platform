namespace Tmobis.Core.Modules;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Cache;
using Tmobis.Core.Converters;
using Tmobis.Core.DI;
using Tmobis.Core.Entities;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Mappers;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Medium.Options;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Passwords;
using Tmobis.Core.Repositories;

/// <summary>
/// Базовый модуль ядра Платформы Т-Мобис.
/// Содержит инфраструктуру кэша, сущностей, репозиториев, обмена сообщениями, хеширования паролей и <c>HttpClient</c>.
/// Подключает consumers из <c>IConsumerModule</c>.
/// </summary>
public sealed class CoreModule : ModuleBase
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);

    RegisterCache(services, configuration);
    RegisterEntities(services);
    RegisterMedium(services, modules, configuration);

    services.AddTransient<IPasswordHasher, PBKDF2PasswordHasher>();
    services.AddTransient<IPropertyNameConverter, PropertyNameConverter>();
    services.AddHttpClient();
  }

  private static void RegisterEntities(IServiceCollection services)
  {
      services.AddTransient<IRepositoryFactory, RepositoryFactory>();
      services.AddSingleton<IEntityRegister, EntityRegister>();
      services.AddSingleton<IEntityMaterializer, EntityMaterializer>();
      services.AddTransient(
          typeof(ICachedEntityProvider<>), typeof(CachedEntityProvider<>));
  }

  private static void RegisterCache(
    IServiceCollection services,
    IConfiguration configuration)
  {
    services.Configure<CacheOptions>(
      configuration.GetSection(CacheOptions.Cache));
    services.AddMemoryCache();
    services.AddOrReplaceTransient<ICacheProvider, MemoryCacheProvider>();
  }

  private static void RegisterMedium(
    IServiceCollection services,
    IModuleRegister modules,
    IConfiguration configuration)
  {
    RegisterConsumersFromModules(services, modules);
    services.AddKeyedTransient<IMessageProducer, InMemoryProducer>("Default");
    services.AddTransient<IMessageDispatcher, MessageDispatcher>();
    services.AddTransient<IMessageRegister, MessageRegister>();
    services.AddSingleton<IProducerTypeRegistry, ProducerTypeRegistry>();
    services.AddTransient<IMessageTypeBinder, MessageTypeBinder>();
    services.AddSingleton<IMessageListRegister, MessageListRegister>();
    services.AddSingleton<IMessageProducerRegister, MessageProducerRegister>();
    services.Configure<MessageListRegisterOptions>(
      configuration.GetSection(MessageListRegisterOptions.MessageOverride));
    services.Configure<MessageProducerOptions>(
      configuration.GetSection(MessageProducerOptions.MessageOverride));
  }

  private static void RegisterConsumersFromModules(
    IServiceCollection services,
    IModuleRegister modules)
  {
    ArgumentNullException.ThrowIfNull(services);
    ArgumentNullException.ThrowIfNull(modules);

    var assemblies = modules
      .OfType<IConsumerModule>()
      .Select(module => module.GetType().Assembly)
      .Distinct();

    var consumerInterfaces = CreateConsumerInterfaceSet();
    var consumerAggregates = new Dictionary<Type, MessageConsumerMapping>();
    
    foreach (var assembly in assemblies)
    {
      RegisterAssemblyConsumers(
        services,
        assembly,
        consumerInterfaces,
        consumerAggregates);
    }

    foreach (var kvp in consumerAggregates)
    {
      services.AddKeyedTransient<MessageConsumerMapping>(
          kvp.Key, (sp, key) => kvp.Value);
    }
  }

  private static HashSet<Type> CreateConsumerInterfaceSet()
    =>
    [
      typeof(IMessageConsumer<>),
      typeof(IMessageConsumer<,>)
    ];

  private static void RegisterAssemblyConsumers(
    IServiceCollection services,
    Assembly assembly,
    HashSet<Type> consumerInterfaces,
    Dictionary<Type, MessageConsumerMapping> consumerAggregates)
  {
    foreach (var implementation in GetConsumerImplementations(assembly))
    {
      RegisterConsumerInterfaces(
        services,
        implementation,
        consumerInterfaces,
        consumerAggregates);
    }
  }

  private static IEnumerable<Type> GetConsumerImplementations(Assembly assembly)
    => GetAssemblyTypes(assembly)
      .Where(type => type.IsClass && !type.IsAbstract);

  private static Type[] GetAssemblyTypes(Assembly assembly)
  {
    try
    {
      return assembly.GetTypes();
    }
    catch (ReflectionTypeLoadException ex)
    {
      return ex.Types.OfType<Type>().ToArray();
    }
  }

  private static void RegisterConsumerInterfaces(
    IServiceCollection services,
    Type implementation,
    HashSet<Type> consumerInterfaces,
    Dictionary<Type, MessageConsumerMapping> consumerAggregates)
  {
    foreach (var serviceType in implementation
      .GetInterfaces()
      .Where(static iface => iface.IsGenericType))
    {
      var typeDefinition = serviceType.GetGenericTypeDefinition();
      if (!consumerInterfaces.Contains(typeDefinition)) continue;
      
      services.AddTransient(serviceType, implementation);

      if (!consumerAggregates.TryGetValue(serviceType, out var set))
      {
          set = [];
          consumerAggregates.Add(serviceType, set);
      }
      set.Add(implementation);
    }
  }
}
