namespace Tmobis.Frameworks.Flexberry.Web.Users;
using Tmobis.Frameworks.Web.Backend.Users;

/// <summary>
/// Обертка вокруг платформенного ICurrentUser для реализации
/// технологического IUser.
/// </summary>
/// <param name="currentUser">Платформенный ICurrentUser.</param>
public class FlexberryCurrentUser(ICurrentUser currentUser)
: NewPlatform.Flexberry.ORM.CurrentUserService.ICurrentUser
{
    private readonly ICurrentUser currentUser = currentUser;

    /// <inheritdoc/>
    public string Login
    {
        get => currentUser.Login;
        set { }
    }

    /// <inheritdoc/>
    public virtual string Domain
    {
        get => string.Empty;
        set { }
    }

    /// <inheritdoc/>
    public virtual string FriendlyName
    {
        get => Login;
        set { }
    }
}
