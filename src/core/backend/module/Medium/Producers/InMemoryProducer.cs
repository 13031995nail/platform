namespace Tmobis.Core.Medium.Producers;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Repositories;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Extensions;

public class InMemoryProducer(IServiceProvider services)
: IMessageProducer, IChainPublishProducer
{
    public virtual async Task<IMessageResult> SendWithResult(IMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        var (provider, scope) = ResolveScope(message);
        using (scope)
        {
            var consumer = GetConsumer(message, provider);
            var result = await SendWithResult(message, consumer);

            if (scope == null) return result;
            
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            await unitOfWork.Commit();

            // TODO: Материализация результата?
            return result;
        }
    }

    public virtual async Task Send(IMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        var (provider, scope) = ResolveScope(message);
        using (scope)
        {
            var consumer = GetConsumer(message, provider);
            await Send(message, consumer);

            if (scope == null) return;
            
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            await unitOfWork.Commit();
        }
    }

    public virtual async Task Publish(IMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (message is IAmbientScopeMessage)
        {
            // Один скоуп = последовательная обработка
            foreach (var consumer in GetConsumers(message, services))
            {
                await Send(message, consumer);
            }

            return;
        }

        // Независимые скоупы = допускается параллельная обработка
        var consumerTypes = GetConsumerTypes(message);
        var tasks = consumerTypes.Select(async consumerType =>
        {
            using var scope = services.CreateScope();
            var scopedConsumer = scope.ServiceProvider.GetRequiredService(consumerType);
            await Send(message, scopedConsumer);
            
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            await unitOfWork.Commit();
        });
        await Task.WhenAll(tasks);
    }

    public virtual async Task<List<IChainMessageResult>> PublishChainWithResult(
        IMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        var (provider, scope) = ResolveScope(message);
        using (scope)
        {
            var consumers = GetConsumers(message, provider);
            List<IChainMessageResult> results = [];
            foreach (var consumer in consumers)
            {
                var result = await SendWithResult(message, consumer)
                    as IChainMessageResult
                    ?? throw new InvalidOperationException(
                        "Неверный тип ответа");

                results.Add(result);
                if (!result.Continue)
                {
                    break;
                }
            }

            if (scope != null)
            {
                var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
                await unitOfWork.Commit();
            }

            return results;
        }
    }

    protected virtual Task Send(
        IMessage message, object consumer, string method = "Consume")
    {
        var methodInfo = consumer.GetType()
        .GetMethod(method,
            BindingFlags.Public | BindingFlags.Instance)
        ?? throw new InvalidOperationException(
             $"{method} не реализован");

        var task = methodInfo.Invoke(consumer, [message]) as Task
            ?? throw new InvalidOperationException(
                $"Неверный возвращаемый тип у {method}");
        return task;
    }

    protected virtual async Task<IMessageResult> SendWithResult(
        IMessage message, object consumer)
    {
        var task = Send(message, consumer, "ConsumeWithResult");
        await task;
        var resultProperty = task.GetType().GetProperty("Result");
        var result = resultProperty?.GetValue(task);
        return result as IMessageResult
            ?? throw new InvalidOperationException(
                "Не удалось получить результат");
    }

    protected virtual (IServiceProvider Provider, IDisposable? Scope) ResolveScope(
        IMessage message)
    {
        if (message is IAmbientScopeMessage)
        {
            return (services, null);
        }

        var scope = services.CreateScope();
        return (scope.ServiceProvider, scope);
    }

    protected virtual object GetConsumer(IMessage message, IServiceProvider provider)
    => GetConsumers(message, provider).FirstOrDefault()
        ?? throw new InvalidOperationException(
            "Не найден Consumer");

    protected virtual List<object> GetConsumers(IMessage message, IServiceProvider provider)
    {
        var consumerInterface = BuildConsumerInterface(message);
        return [.. provider
            .GetServices(consumerInterface)
            .Where(service => service != null)
            .Cast<object>()];
    }

    protected virtual List<Type> GetConsumerTypes(IMessage message)
    {
        var consumerInterface = BuildConsumerInterface(message);
        var mapping = services.GetKeyedService<MessageConsumerMapping>(consumerInterface);
        return mapping is null ? [] : [.. mapping];
    }

    protected virtual Type BuildConsumerInterface(IMessage message)
    {
        var messageType = message.GetType();
        if (!messageType.IsAssignableTo(typeof(IMessage)))
        {
            throw new ArgumentException(
                $"Неверный тип {nameof(messageType)}");
        }

        var consumerType = typeof(IMessageConsumer<>);
        List<Type> typeArgs = [messageType];

        var genericMessageInterface = message.GetMessageResultType();
        if (genericMessageInterface != null)
        {
            typeArgs.Add(genericMessageInterface);
            consumerType = typeof(IMessageConsumer<,>);
        }

        return consumerType.MakeGenericType([.. typeArgs]);
    }
}
