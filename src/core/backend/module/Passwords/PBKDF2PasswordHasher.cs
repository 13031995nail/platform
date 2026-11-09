namespace Tmobis.Core.Passwords;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Сервис хэширования паролей.
/// </summary>
public class PBKDF2PasswordHasher
: PasswordHasher<object>, IPasswordHasher
{
  /// <summary>
  /// Получить хеш для указанного пароля.
  /// </summary>
  /// <param name="password">Пароль пользователя в открытом виде.</param>
  /// <returns>Хеш пароля.</returns>
  public string Hash(string password) => HashPassword(null, password);

  /// <summary>
  /// Проверить, совпадает ли предоставленный пароль с эталонным хэшем.
  /// </summary>
  /// <param name="hashedPassword">Хэш из хранилища.</param>
  /// <param name="providedPassword">Проверяемый пароль.</param>
  /// <returns></returns>
  public bool VerifyPassword(string hashedPassword, string providedPassword)
  => VerifyHashedPassword(null, hashedPassword, providedPassword) != PasswordVerificationResult.Failed;
}
