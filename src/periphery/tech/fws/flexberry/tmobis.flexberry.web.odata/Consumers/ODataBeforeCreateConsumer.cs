namespace Tmobis.Frameworks.Flexberry.Web.OData.Consumers
{
    using Tmobis.Core.Medium.Consumers;
    using Tmobis.Frameworks.Flexberry.Web.OData.Messages;

    public class ODataBeforeCreateConsumer : IMessageConsumer<ODataBeforeCreateMessage>
    {
        public async Task Consume(ODataBeforeCreateMessage message)
        {
            // TODO: Перенести
            // if (request.Object is ApplicationLog log)
            // {
            //     log.UserName = this.currentUserSerivce.GetCurrentUser()?.Login;
            // }
        }
    }
}
