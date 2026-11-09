namespace Tmobis.Frameworks.Flexberry.Web.OData.Consumers
{
    using System.Linq;
    using ICSSoft.STORMNET;
    using ICSSoft.STORMNET.Business;
    using ICSSoft.STORMNET.FunctionalLanguage;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Tmobis.Business.Core.Providers.Roles;
    using Tmobis.Business.Core.Providers.Users;
    using Tmobis.Core.Medium.Consumers;
    using Tmobis.Frameworks.Flexberry.Web.OData.Access;
    using Tmobis.Frameworks.Flexberry.Web.OData.Messages;
    using User = Business.Core.Flexberry.Models.User;

    public class ODataBeforeGetConsumer : IMessageConsumer<ODataBeforeGetMessage>
    {
        private readonly ICurrentUserProvider currentUserService;
        private readonly ICurrentRoleProvider currentRoleProvider;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IDataService dataService;
        private readonly IEnumerable<IODataReadRestrictionProvider> restrictionProviders;


        public ODataBeforeGetConsumer(
            ICurrentUserProvider currentUserService,
            ICurrentRoleProvider currentRoleProvider,
            IHttpContextAccessor httpContextAccessor,
            IDataService dataService,
            IEnumerable<IODataReadRestrictionProvider> restrictionProviders)
        {
            this.currentUserService = currentUserService;
            this.currentRoleProvider = currentRoleProvider;
            this.httpContextAccessor = httpContextAccessor ??
          throw new ArgumentNullException(nameof(httpContextAccessor));
            this.dataService = dataService;
            this.restrictionProviders = restrictionProviders;
        }

        public async Task Consume(ODataBeforeGetMessage message)
        {
            if (message.Lcs.View.DefineClassType.Name == nameof(User))
            {
                User currentUser = (User)await currentUserService.GetCurrentUser();
                var currentUserLogin = currentUser?.Login;

                var contextUrl = httpContextAccessor.HttpContext?.Request.GetEncodedUrl();

                if (currentUserLogin == null)
                {
                    throw new Exception("Неавторизованный доступ к пользователю");
                }

                if (message.Lcs.LimitFunction == null)
                {
                    throw new Exception("Необходимо указать фильтр для запроса к пользователям");
                }

                var roles = await currentRoleProvider.GetCurrentRoleNames();

                bool isMassGetting = !(message.Lcs.LimitFunction.GetLimitProperties().Contains("STORMMainObjectKey")
                                    || message.Lcs.LimitFunction.GetLimitProperties().Contains("UserId"));

                if (isMassGetting)
                {
                    if (!(roles.Contains("admin") || roles.Contains("Администратор")))
                    {
                        throw new Exception("Недостаточно полномочий для массового получения пользователей");
                    }

                    var allowedProps = new[] { "Name", "UserId", "Login", "Enabled", "IsUser", "IsGroup", "IsRole", "Town", "Town.Name", "Email", "Status", "CompanyGroup", "Organization", "ProductionUnit" };
                    message.Lcs.View.Properties = message.Lcs.View.Properties.Where(prop => allowedProps.Contains(prop.Name)).ToArray();

                    if (message.Lcs.ReturnTop > 100)
                    {
                        var logMessage = $"Выгрузка {message.Lcs.ReturnTop} пользователей. Пользователь: {currentUserLogin}. Запрос: {contextUrl} строк.";
                        LogService.LogWarn(logMessage);
                    }
                    else if (message.Lcs.ReturnTop == 0)
                    {
                        message.Lcs.ReturnTop = 100;
                    }
                }
                else
                {
                    var searchingUser = dataService.LoadObjects(message.Lcs).Cast<User>().FirstOrDefault();

                    if (searchingUser != null && searchingUser.UserId == null)
                    {
                        dataService.LoadObject(User.Views.UserE, searchingUser);
                    }

                    if (!(roles.Contains("admin") || roles.Contains("Администратор") || searchingUser?.__PrimaryKey?.ToString() == currentUser?.__PrimaryKey?.ToString() || (searchingUser?.UserId != null && searchingUser?.UserId.ToString() == currentUser?.UserId?.ToString())))
                    {
                        throw new Exception("Недостаточно полномочий для получения пользователя");
                    }
                    message.Lcs.ReturnTop = 1;
                }
            }

            if (message.Lcs?.View?.DefineClassType == null)
            {
                return;
            }

            var entityType = message.Lcs.View.DefineClassType;
            var restrictions = new List<ODataReadRestriction>();
            foreach (var restrictionProvider in restrictionProviders.Where(provider =>
                provider.Supports(entityType)))
            {
                restrictions.Add(await restrictionProvider.GetRestriction(entityType));
            }

            foreach (var propertyPath in restrictions
                .SelectMany(restriction => restriction.RequiredProperties)
                .Distinct(StringComparer.Ordinal)
                .Where(propertyPath => !message.Lcs.View.CheckPropname(propertyPath)))
            {
                message.Lcs.View.AddProperty(propertyPath);
            }

            foreach (var limitFunction in restrictions
                .Select(restriction => restriction.LimitFunction)
                .Where(limitFunction => limitFunction != null))
            {
                message.Lcs.LimitFunction = FunctionBuilder.BuildAnd(
                    message.Lcs.LimitFunction,
                    limitFunction);
            }
        }
    }
}
