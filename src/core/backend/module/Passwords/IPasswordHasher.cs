namespace Tmobis.Core.Passwords;

/// <summary>
/// Сервис хэширования паролей.
/// </summary>
public interface IPasswordHasher
{
  /// <summary>
  /// Получить хеш для указанного пароля.
  /// </summary>
  /// <param name="password">Пароль пользователя в открытом виде.</param>
  /// <returns>Хеш пароля.</returns>
  string Hash(string password);

  /// <summary>
  /// Проверить, совпадает ли предоставленный пароль с эталонным хэшем.
  /// </summary>
  /// <param name="hashedPassword">Хэш из хранилища.</param>
  /// <param name="providedPassword">Проверяемый пароль.</param>
  /// <returns></returns>
  bool VerifyPassword(string hashedPassword, string providedPassword);
}
