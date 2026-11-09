namespace Tmobis.Frameworks.Flexberry.Web.OData.Consumers
{
    using Tmobis.Core.Medium.Consumers;
    using Tmobis.Frameworks.Flexberry.Web.OData.Messages;

    public class ODataAfterCreateConsumer : IMessageConsumer<ODataAfterCreateMessage>
    {
        public async Task Consume(ODataAfterCreateMessage message)
        {
            // TODO: Перенести
            // Поле ApplicationLog.Number на уровне БД - автоинкремент.
            // Его нужно дочитать самостоятельно.
            // if (request.Object is ApplicationLog log)
            // {
            //     var view = new View
            //     {
            //         DefineClassType = typeof(ApplicationLog),
            //     };
            //     view.AddProperty(nameof(ApplicationLog.Number));
            //     DataServiceProvider.DataService.LoadObject(log, false, false);
            // }
        }
    }
}
