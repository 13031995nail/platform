namespace Tmobis.Notification.Send.Rabbit.Consumers;

using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Producers;
using Tmobis.Notification.Send.Rabbit.Models.Messages;

public class SendRabbitNotificationConsumer(
    IMessageDispatcher messageDispatcher)
    : IMessageConsumer<SendRabbitNotification>
{
    public async Task Consume(SendRabbitNotification message)
    {
        await messageDispatcher.Send(new CreateNotification
        {
            UserId = new Guid("29c872af-7e61-43bb-8893-7f91b5ab71bd"),
            TypeId = new Guid("e23e1885-87dc-4e80-a7a4-a0a862191f1b"),
            Content = new List<CreateNotificationContent>
            {
                new CreateNotificationContent {
                    ProviderTypeId = new Guid("64a0cb5b-30e6-4426-98d6-e6a40e3e0831"),
                    Title = "Test",
                    Content = "Test"
                },
            }
        });
    }
}
