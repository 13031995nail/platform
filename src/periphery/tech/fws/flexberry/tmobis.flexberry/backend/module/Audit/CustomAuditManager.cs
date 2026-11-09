namespace Tmobis.Frameworks.Flexberry.Audit;

using System.Net;
using ICSSoft.STORMNET.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NewPlatform.Flexberry.AuditBigData;

public class CustomAuditManager : LegacyAuditManager
{
  private readonly IHttpContextAccessor httpContextAccessor;

  public CustomAuditManager(
    [FromKeyedServices("Audit")] IDataService dataService,
    ILegacyAuditConverter auditConverter,
    ILegacyAuditSerializer auditSerializer,
    IHttpContextAccessor httpContextAccessor)
    : base(dataService, auditConverter, auditSerializer)
  {
    this.httpContextAccessor = httpContextAccessor ??
          throw new ArgumentNullException(nameof(httpContextAccessor));
  }

  /// <inheritdoc/>
  protected override AuditRecord CreatePrimaryAuditRecord(
      string userName, string userLogin, string objectType,
      object objectPrimaryKey, DateTime operationTime,
      string operationType, ExecutionStatus executionStatus,
      string source, string serializedFields, Guid? auditEntityGuid)
  {
    string ip = GetClientIpAddress();
    if (!string.IsNullOrEmpty(ip))
    {
      source = $"{ip}; {source}";
    }

    return base.CreatePrimaryAuditRecord(
        userName, userLogin, objectType, objectPrimaryKey,
        operationTime, operationType, executionStatus,
        source, serializedFields, auditEntityGuid);
  }

  protected virtual string? GetClientIpAddress()
  {
    try
    {
      var httpContext = httpContextAccessor.HttpContext;
      if (httpContext == null)
      {
        return null;
      }

      var connection = httpContext.Connection;
      string? ipAddress = null;

      var remoteIp = connection.RemoteIpAddress;
      if (remoteIp != null)
      {
        if (remoteIp.Equals(System.Net.IPAddress.IPv6Loopback))
        {
          ipAddress = "127.0.0.1";
        }
        else
        {
          ipAddress = remoteIp.ToString();
        }
      }

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
        throw new Exception("Ошибка получения DNS пользователя");
      }

      return $"IP: {ipAddress}";
    }
    catch
    {
      return null;
    }
  }
}
