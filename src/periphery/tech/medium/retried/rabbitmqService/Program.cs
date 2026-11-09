using Microsoft.Extensions.Hosting;
using Tmobis.Core.Application;
using Tmobis.Core.Modules;
using Tmobis.Jobs.Quartz;
using Tmobis.Medium.Delayed.InRepo.Module;
using Tmobis.Medium.RabbitMQ;
using Tmobis.Medium.Retried.RabbitMQ;
using Tmobis.Medium.Retried.RabbitMQ.Service.Modules;
using Tmobis.Medium.Retried.WithDelay.Module;
using Tmobis.Core.Medium.Modules;


var modules = new ModuleRegister([
    new CoreModule(),
    new QuartzAdapterModule(),
    new RabbitAdapterModule(),
    new RabbitProducerModule(),
    new MessageListenerWorkerModule(),
    new DLQRabbitListenerModule(),
    new RabbitMQDLQModule(),
    new DelayedMessageNowConsumerModule(),
    new DelayedMessageTickConsumerModule(),
    new RetriedMessageConsumerModule(),
    new DelayedMessageCreatedRepositoryProducerModule(),
]);

var appBuilder = new WorkerAppBuilder(modules, args);
var app = appBuilder.Build();
await app.RunAsync();
