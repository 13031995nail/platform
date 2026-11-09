namespace Tmobis.Frameworks.Web.Backend;

using System.Net;
using Microsoft.AspNetCore.Http;

public class HttpContextIpProvider(
    IHttpContextAccessor httpContextAccessor)
: IHttpContextIpProvider
{
    /// <inheritdoc/>
    public string? GetCurrentConnectionAddress()
    {
        var ip = GetRawIp();
        if (ip == null)
            return null;
        
        return ip.Equals(IPAddress.IPv6Loopback)
            ? "127.0.0.1"
            : ip.ToString();
    }
    
    private IPAddress? GetRawIp()
    {
        var context = httpContextAccessor.HttpContext;
        if (context == null)
        {
            return null;
        }

        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            foreach (var headerValue in forwardedFor.Split(','))
            {
                var parsedForwarded = TryParseIpAddress(headerValue);
                if (parsedForwarded != null)
                {
                    return parsedForwarded;
                }
            }
        }

        var realIp = TryParseIpAddress(context.Request.Headers["X-Real-IP"].FirstOrDefault());
        return realIp ?? context.Connection.RemoteIpAddress;
    }
  
    private static IPAddress? TryParseIpAddress(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();

        if (IPAddress.TryParse(trimmed, out var ipAddress))
        {
            return ipAddress;
        }

        if (IPEndPoint.TryParse(trimmed, out var endPoint))
        {
            return endPoint.Address;
        }

        var lastColonIndex = trimmed.LastIndexOf(':');
        if (lastColonIndex > 0)
        {
            var addressPart = trimmed.Substring(0, lastColonIndex);
            if (IPAddress.TryParse(addressPart, out ipAddress))
            {
                return ipAddress;
            }
        }

        return null;
    }
}
