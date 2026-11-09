namespace Tmobis.Frameworks.Flexberry.Web.OData.Consumers;

using System.Linq;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Business.Core.Providers.Users;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Frameworks.Flexberry.Web.OData.Messages;
using User = Business.Core.Flexberry.Models.User;

public class ODataBeforeUpdateConsumer : IMessageConsumer<ODataBeforeUpdateMessage>
{
    private readonly ICurrentUserProvider currentUserService;
    private readonly ICurrentRoleProvider currentRoleProvider;

    public ODataBeforeUpdateConsumer(
        ICurrentUserProvider currentUserService,
        ICurrentRoleProvider currentRoleProvider)
    {
        this.currentUserService = currentUserService;
        this.currentRoleProvider = currentRoleProvider;
    }

    public async Task Consume(ODataBeforeUpdateMessage message)
    {
        if (message.Object is not User user)
        {
            return;
        }

        User currentUser = (User)await currentUserService.GetCurrentUser();
        var alteredProps = user.GetAlteredPropertyNames();

        if (currentUser?.Login == null)
        {
            throw new Exception("Неавторизованный доступ");
        }

        var currentUserRoles = await currentRoleProvider.GetCurrentRoleNames();
        var allowedRoles = new[] { "admin", "Администратор" };

        if (alteredProps.Any()
            && !(currentUserRoles.Intersect(allowedRoles).Any()
                || user.__PrimaryKey?.ToString() == currentUser.__PrimaryKey?.ToString()))
        {
            throw new Exception("Недостаточно полномочий для изменения пользователя");
        }

        var notAllowedProps = new[]
        {
            "Login", "IsUser", "IsGroup", "IsRole", "ConnString",
            "Enabled", "Town", "UserId", "PwdEditTime", "Pwd1",
            "Pwd2", "Status", "CompanyGroup", "Organization",
            "ProductionUnit", "IsComplexPwd", "UserPwd"
        };

        var badProps = alteredProps.Intersect(notAllowedProps);

        if (badProps.Any())
        {
            throw new Exception("Запрещено изменять поля: " + string.Join(",", badProps));
        }
    }
}
