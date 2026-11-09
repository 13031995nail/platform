namespace Tmobis.Auth.Password.Services;

using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Options;
using Tmobis.Business.Core.Models.Users;

/// <summary>
/// Сервис таймаута авторизации.
/// </summary>
public class AuthTimeoutService : IAuthTimeoutService
{
  private readonly IMemoryCache memoryCache;
  private readonly AuthOptions options;
  private readonly int triesLimit;
  private readonly int blockTime;
  private readonly int expirationTime;

  public AuthTimeoutService(
    IOptions<AuthOptions> optionsParam,
    IMemoryCache memoryCacheParam)
  {
    memoryCache = memoryCacheParam;
    options = optionsParam.Value;

    triesLimit = int.Parse(options.Timeouts.TriesLimit);
    blockTime = int.Parse(options.Timeouts.BlockTimeMinutes);
    expirationTime = int.Parse(options.Timeouts.ExpirationTimeMinutes);
  }

  /// <summary>
  /// Увеличение количества неудачных попыток авторизации.
  /// </summary>
  /// <param name="user">Авторизуемый пользователь.</param>
  /// <returns>Флаг, выводить ли сообщение о блокировке.</returns>
  public bool IncreaseTriesNumber(IUser user)
  {
    if (user == null)
    {
      return false;
    }

    var hasRecord = memoryCache.TryGetValue(user.Login, out int triesNumber);

    if (triesNumber == triesLimit)
    {
      memoryCache.Set(user.Login, triesNumber, TimeSpan.FromMinutes(blockTime));
      return true;
    }

    triesNumber += 1;

    memoryCache.Set(user.Login, triesNumber, TimeSpan.FromMinutes(expirationTime));
    return false;
  }

  /// <summary>
  /// Заблокирована ли авторизация для учетной записи.
  /// </summary>
  /// <param name="user">Авторизуемый пользователь.</param>
  /// <returns>Флаг, заблокирована ли возможность входа.</returns>
  public bool IsBlocked(IUser user)
  {
    if (user == null)
    {
      return false;
    }

    var hasRecord = memoryCache.TryGetValue(user.Login, out int triesNumber);

    if (triesNumber == triesLimit) { return true; }

    return false;
  }
}
