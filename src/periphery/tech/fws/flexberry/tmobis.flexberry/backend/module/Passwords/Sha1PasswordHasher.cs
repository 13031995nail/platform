namespace Tmobis.Frameworks.Flexberry.Passwords;
using Microsoft.AspNetCore.Identity;
using NewPlatform.Flexberry.Security;
using System.Security.Cryptography;
using Tmobis.Core.Passwords;

/// <summary>
/// Сервис хэширования паролей.
/// </summary>
public class Sha1PasswordHasher : BasePasswordHasher
{
  /// <summary>
  /// ///     <see cref="Sha1PasswordHasher"/>.
  /// </summary>
  public Sha1PasswordHasher() : base()
  {

  }

  /// <summary>
  /// ///     <see cref="Sha1PasswordHasher"/>.
  /// </summary>
  /// <param name="salt">   .</param>
  public Sha1PasswordHasher(string salt) : base(salt)
  {

  }

  /// <summary>
  ///   SHA1   .
  /// </summary>
  /// <returns>.NET-  SHA1.</returns>
  protected override HashAlgorithm GetHashAlgorithm()
  {
    return SHA1.Create();
  }

  /// <summary>
  ///   SHA1       .
  /// </summary>
  /// <returns>.NET-  SHA1    .</returns>
  protected override HMAC GetHashAlgorithmWithKey()
  {
    return new HMACSHA1();
  }
}
