namespace Tmobis.Frameworks.Flexberry.Web.OData.Consumers
{
    using ICSSoft.STORMNET.Security;
    using Tmobis.Business.Core.Models.Users;
    using Tmobis.Core.Medium.Consumers;
    using Tmobis.Frameworks.Flexberry.Web.OData.Messages;

    public class ODataAfterGetConsumer : IMessageConsumer<ODataAfterGetMessage>
    {
        public async Task Consume(ODataAfterGetMessage message)
        {
            foreach (var obj in message.Objects)
            {
                if (obj as Agent != null)
                {
                    ((Agent)obj).Pwd = null;
                }

                if (obj as IUser != null)
                {
                    ((IUser)obj).UserPwd = null;
                }
            }
        }
    }
}
