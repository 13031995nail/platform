namespace Tmobis.Frameworks.Flexberry.Passwords;
using Tmobis.Core.Passwords;

/// <summary>
/// Сервис хэширования паролей.
/// </summary>
public class FlexberryPBKDF2PasswordHasher
: PBKDF2PasswordHasher, NewPlatform.Flexberry.Security.IPasswordHasher
{
    /// <inheritdoc/>
    public string Hash(string password, string salt)
    => throw new NotImplementedException("Функция не реализована.");
}