namespace Tmobis.Backend.EF;

using Core.DI;
using Core.Modules;
using Notification.WebSocket;
using Tmobis.Core.Medium.Modules;
using Tmobis.Notification;

public class TestModule:
    ModuleBase,
    IMessageModule
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddListenedMessage<TestMessage>("WebApi");
        services.AddProducerMessage<SendNotification>("SignalRProducer");
        services.AddListenedMessage<SendNotification>("RabbitMQ");
    }
}