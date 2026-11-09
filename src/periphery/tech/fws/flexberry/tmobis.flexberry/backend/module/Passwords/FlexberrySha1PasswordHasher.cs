namespace Tmobis.Frameworks.Flexberry.Passwords;

/// <summary>
/// Сервис хэширования паролей.
/// </summary>
public class FlexberrySha1PasswordHasher
: Sha1PasswordHasher, NewPlatform.Flexberry.Security.IPasswordHasher, Tmobis.Core.Passwords.IPasswordHasher
{
  /// <summary>
  /// Проверить, совпадает ли предоставленный пароль с эталонным хэшем.
  /// </summary>
  /// <param name="hashedPassword">Хэш из хранилища.</param>
  /// <param name="providedPassword">Проверяемый пароль.</param>
  /// <returns></returns>
  public bool VerifyPassword(string hashedPassword, string providedPassword)
  => string.Equals(Hash(providedPassword), hashedPassword, StringComparison.OrdinalIgnoreCase);
}
