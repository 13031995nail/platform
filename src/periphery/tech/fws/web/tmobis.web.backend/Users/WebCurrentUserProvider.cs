namespace Tmobis.Frameworks.Web.Backend.Users;

using System.Net;
using Tmobis.Business.Core.Models.Employees;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Business.Core.Providers.Employees;
using Tmobis.Business.Core.Providers.Users;

/// <summary>
/// Реализация получения текущего пользователя в веб-приложении.
/// </summary>
public class WebCurrentUserProvider(
  ICurrentUser currentUser,
  IUserProvider userProvider,
  IEmployeeProvider employeeProvider,
  IHttpContextIpProvider ipProvider)
: ICurrentUserProvider
{
  /// <inheritdoc/>
  public virtual async Task<IUser?> GetCurrentUser()
  {
      var login = currentUser.Login;
      return string.IsNullOrEmpty(login)
        ? null
        : await userProvider.GetUserByLogin(login);
  }

  /// <inheritdoc/>
  public virtual async Task<IEmployee?> GetCurrentEmployee()
  {
    var user = await GetCurrentUser();
    return user == null
    ? null
    : await employeeProvider.GetByUser(user);
  }

  /// <inheritdoc/>
  public virtual string? GetUserConnectionInfo()
  {
      try
      {
          var ipAddress = ipProvider
              .GetCurrentConnectionAddress();

          if (string.IsNullOrEmpty(ipAddress))
          {
              return null;
          }

          try
          {
              var hostEntry = Dns.GetHostEntry(ipAddress);
              var dnsName = hostEntry.HostName;

              if (!string.IsNullOrEmpty(dnsName) && dnsName != ipAddress)
              {
                  return $"IP: {ipAddress}; DNS: {dnsName}";
              }
          }
          catch (Exception)
          {
              // ignored
          }

          return $"IP: {ipAddress}";
      }
      catch
      {
          return null;
      }
  }

  /// <inheritdoc/>
  public string? GetUserConnectionAddress()
  => ipProvider.GetCurrentConnectionAddress();
}