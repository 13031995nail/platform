namespace Tmobis.Business.Core.Providers.Users;
using Tmobis.Business.Core.Models.Employees;
using Tmobis.Business.Core.Models.Users;

/// <summary>
/// Пустая реализация сервиса текущего пользователя.
/// </summary>
public class EmptyCurrentUserProvider
: ICurrentUserProvider
{
  /// <inheritdoc/>
  public Task<IUser?> GetCurrentUser()
  => Task.FromResult<IUser?>(null);

  /// <inheritdoc/>
  public Task<IEmployee?> GetCurrentEmployee()
  => Task.FromResult<IEmployee?>(null);

  public string? GetUserConnectionInfo()
  => null;

  public string? GetUserConnectionAddress()
    => null;

}