namespace Tmobis.Medium.Signalr.ModuleOptions;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

public class SignalRJwtBearerEvents : IPostConfigureOptions<JwtBearerOptions>
{

  public SignalRJwtBearerEvents(IConfiguration configuration)
  {
  }

  public void PostConfigure(string name, JwtBearerOptions options)
  {
    options.Events = options.Events ?? new JwtBearerEvents();

    var previousHandler = options.Events.OnMessageReceived;

    options.Events.OnMessageReceived = async context =>
    {
      var accessToken = context.Request.Query["access_token"];
      var path = context.HttpContext.Request.Path;

      if (!string.IsNullOrEmpty(accessToken) &&
          path.StartsWithSegments("/signalr"))
      {
        context.Token = accessToken;
      }

      if (previousHandler != null)
      {
        await previousHandler(context);
      }
    };
  }
}
