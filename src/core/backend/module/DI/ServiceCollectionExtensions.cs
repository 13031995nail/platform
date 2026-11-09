namespace Tmobis.Core.DI;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Medium.Mappers;

/// <summary>
/// Вспомогательные методы для работы с <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавить или заменить сервис, если он уже существует.
    /// </summary>
    /// <typeparam name="TI">Интерфейс сервиса.</typeparam>
    /// <typeparam name="TC">Реализация сервиса.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    public static void AddOrReplaceScoped<TI, TC>(
        this IServiceCollection services)
    where TI : class
    where TC : class, TI
    {
        services.RemoveServicesIfExists<TI>();
        services.AddScoped<TI, TC>();
    }

    public static IServiceCollection AddOrReplaceScoped<TI, TC>(
        this IServiceCollection services,
        Func<IServiceProvider, TC> implementationFactory)
    where TI : class
    where TC : class, TI
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(implementationFactory);

        services.RemoveServicesIfExists<TI>();
        return services.AddScoped<TI>(implementationFactory);
    }

    /// <summary>
    /// Добавить или заменить сервис, если он уже существует.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    public static void AddOrReplaceScoped(
        this IServiceCollection services,
        Type interfaceType,
        Func<IServiceProvider, object> implementationFactory)
    {
        services.RemoveServicesIfExists(interfaceType);
        services.AddScoped(interfaceType, implementationFactory);
    }

    /// <summary>
    /// Добавить или заменить сервис, если он уже существует.
    /// </summary>
    /// <typeparam name="TI">Интерфейс сервиса.</typeparam>
    /// <typeparam name="TC">Реализация сервиса.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    public static void AddOrReplaceTransient<TI, TC>(
        this IServiceCollection services)
    where TI : class
    where TC : class, TI
    {
        services.RemoveServicesIfExists<TI>();
        services.AddTransient<TI, TC>();
    }

    /// <summary>
    /// Добавить или заменить сервис, если он уже существует.
    /// </summary>
    /// <typeparam name="TC">Реализация сервиса.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    public static void AddOrReplaceTransient<TC>(
        this IServiceCollection services)
    where TC : class
    {
        services.RemoveServicesIfExists<TC>();
        services.AddTransient<TC>();
    }

    /// <summary>
    /// Добавить или заменить сервис, если он уже существует.
    /// </summary>
    /// <typeparam name="TC">Реализация сервиса.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    public static void AddOrReplaceTransient<TC>(
        this IServiceCollection services,
        Func<IServiceProvider, TC> implementationFactory)
    where TC : class
    {
        services.RemoveServicesIfExists<TC>();
        services.AddTransient(implementationFactory);
    }

    /// <summary>
    /// Добавить или заменить сервис, если он уже существует.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    public static void AddOrReplaceTransient(
        this IServiceCollection services,
        Type interfaceType,
        Type implementationType)
    {
        services.RemoveServicesIfExists(interfaceType);
        services.AddTransient(interfaceType, implementationType);
    }

    /// <summary>
    /// Добавить или заменить сервис, если он уже существует.
    /// </summary>
    /// <typeparam name="TI">Интерфейс сервиса.</typeparam>
    /// <typeparam name="TC">Реализация сервиса.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    public static void AddOrReplaceSingleton<TI, TC>(
        this IServiceCollection services)
    where TI : class
    where TC : class, TI
    {
        services.RemoveServicesIfExists<TI>();
        services.AddSingleton<TI, TC>();
    }

    /// <summary>
    /// Добавить или заменить сервис, если он уже существует.
    /// </summary>
    /// <typeparam name="TI">Интерфейс сервиса.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    public static void AddOrReplaceSingleton<TI>(
        this IServiceCollection services,
        Func<IServiceProvider, TI> implementationFactory)
    where TI : class
    {
        services.RemoveServicesIfExists<TI>();
        services.AddSingleton(implementationFactory);
    }

    /// <summary>
    /// Добавить или заменить сервис, если он уже существует.
    /// </summary>
    /// <typeparam name="TI">Интерфейс сервиса.</typeparam>
    /// <typeparam name="TC">Реализация сервиса.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="serviceKey">Ключ сервиса.</param>
    public static void AddOrReplaceKeyedTransient<TI, TC>(
        this IServiceCollection services, object? serviceKey)
    where TI : class
    where TC : class, TI
    {
        services.RemoveKeyedServicesIfExists<TI>(serviceKey);
        services.AddKeyedTransient<TI, TC>(serviceKey);
    }

    /// <summary>
    /// Добавить или заменить сервис, если он уже существует.
    /// </summary>
    /// <typeparam name="TC">Реализация сервиса.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="serviceKey">Ключ сервиса.</param>
    public static void AddOrReplaceKeyedTransient<TC>(
        this IServiceCollection services, object? serviceKey)
    where TC : class
    {
        services.RemoveKeyedServicesIfExists<TC>(serviceKey);
        services.AddKeyedTransient<TC>(serviceKey);
    }

    /// <summary>
    /// Добавить или заменить сервис, если он уже существует.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="implementationType">Тип реализации.</param>
    /// <param name="serviceKey">Ключ сервиса.</param>
    public static void AddOrReplaceKeyedTransient(
        this IServiceCollection services,
        Type implementationType,
        object? serviceKey)
    {
        services.RemoveKeyedServicesIfExists(implementationType, serviceKey);
        services.AddKeyedTransient(implementationType, serviceKey);
    }

    /// <summary>
    /// Добавить маппинг сообщения на слушатель.
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="listenerName">Имя слушателя.</param>
    public static void AddListenedMessage<TMessage>(
        this IServiceCollection services,
        string listenerName)
    {
        services.AddSingleton(new MessageListenerMapping { MessageType = typeof(TMessage), Name = listenerName });
    }

    /// <summary>
    /// Добавить маппинг сообщения на слушатель.
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="listenerName">Имя слушателя.</param>
    public static void AddProducerMessage<TMessage>(
        this IServiceCollection services,
        string listenerName)
    {
        services.AddSingleton(new MessageProducerMapping { MessageType = typeof(TMessage), Name = listenerName });
    }

    /// <summary>
    /// Добавить или заменить маппинг сообщения на слушатель.
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="listenerName">Имя слушателя.</param>
    public static void AddOrReplaceListenedMessage<TMessage>(
        this IServiceCollection services,
        string listenerName)
    {
        var existingServices = services
            .Where(es => es.ImplementationInstance is MessageListenerMapping mlm && mlm.MessageType == typeof(TMessage))
            .ToList();
        foreach (var existingService in existingServices)
        {
            services.Remove(existingService);
        }

        services.AddSingleton(new MessageListenerMapping { MessageType = typeof(TMessage), Name = listenerName });
    }

    /// <summary>
    /// Добавить или заменить маппинг сообщения на слушатель.
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="listenerName">Имя слушателя.</param>
    public static void AddOrReplaceProducerMessage<TMessage>(
        this IServiceCollection services,
        string listenerName)
    {
        var existingServices = services
            .Where(es => es.ImplementationInstance is MessageProducerMapping mlm && mlm.MessageType == typeof(TMessage))
            .ToList();
        foreach (var existingService in existingServices)
        {
            services.Remove(existingService);
        }

        services.AddSingleton(new MessageProducerMapping { MessageType = typeof(TMessage), Name = listenerName });
    }

    /// <summary>
    /// Удалить сервисы, если они уже существуют.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <typeparam name="TI">Интерфейс сервиса.</typeparam>
    public static void RemoveServicesIfExists<TI>(
        this IServiceCollection services)
    {
        var existingService = services.GetService<TI>();
        if (existingService != null)
        {
            services.Remove(existingService);
        }
    }

    /// <summary>
    /// Удалить сервисы, если они уже существуют.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    public static void RemoveServicesIfExists(
        this IServiceCollection services, Type type)
    {
        var existingServices = services.GetServices(type);
        foreach (var existingService in existingServices)
        {
            services.Remove(existingService);
        }
    }

    /// <summary>
    /// Удалить ключевые сервисы, если они уже существуют.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <typeparam name="TI">Интерфейс сервиса.</typeparam>
    /// <param name="serviceKey">Ключ сервиса.</param>
    public static void RemoveKeyedServicesIfExists<TI>(
        this IServiceCollection services, object? serviceKey)
    where TI : class
    {
        var keyedServices = services
            .Where(sd => sd.ServiceType == typeof(TI) && sd.ServiceKey == serviceKey)
            .ToList();
        foreach (var keyedService in keyedServices)
        {
            services.Remove(keyedService);
        }
    }

    /// <summary>
    /// Удалить ключевые сервисы, если они уже существуют.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="serviceKey">Ключ сервиса.</param>
    public static void RemoveKeyedServicesIfExists(
        this IServiceCollection services, Type type, object? serviceKey)
    {
        var keyedServices = services
            .Where(sd => sd.ServiceType == type && sd.ServiceKey == serviceKey)
            .ToList();
        foreach (var keyedService in keyedServices)
        {
            services.Remove(keyedService);
        }
    }

    /// <summary>
    /// Найти описание зарегистрированного сервиса по интерфейсу.
    /// </summary>
    /// <typeparam name="TI">Интерфейс сервиса.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    /// <returns>Описание зарегистированного сервиса.</returns>
    public static ServiceDescriptor? GetService<TI>(
          this IServiceCollection services) =>
      services.GetService(typeof(TI));

    /// <summary>
    /// Найти описание зарегистрированного сервиса по интерфейсу.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <returns>Описание зарегистированного сервиса.</returns>
    public static ServiceDescriptor? GetService(
          this IServiceCollection services, Type type) =>
      services.FirstOrDefault(d => d.ServiceType == type);

    /// <summary>
    /// Найти описания зарегистрированных сервисов по интерфейсу.
    /// </summary>
    /// <typeparam name="TI">Интерфейс сервиса.</typeparam>
    /// <param name="services">Коллекция сервисов.</param>
    /// <returns>Описания зарегистированных сервисов.</returns>
    public static List<ServiceDescriptor> GetServices<TI>(
          this IServiceCollection services) =>
      services.GetServices(typeof(TI));

    /// <summary>
    /// Найти описания зарегистрированных сервисов по интерфейсу.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <returns>Описания зарегистированных сервисов.</returns>
    public static List<ServiceDescriptor> GetServices(
          this IServiceCollection services, Type type) =>
      [..services.Where(d => d.ServiceType == type)];
}
