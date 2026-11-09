namespace Tmobis.Frameworks.Web.Backend.Users;

/// <summary>
/// Текущий веб-пользователь.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Логин пользователя ("vpupkin").
    /// </summary>
    string Login { get; }
}