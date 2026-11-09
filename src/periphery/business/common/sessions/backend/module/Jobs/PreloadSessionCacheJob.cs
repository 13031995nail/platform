namespace Tmobis.Business.Common.Sessions.Jobs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

public class PreloadSessionCacheJob(
  ILogger<PreloadSessionCacheJob> logger)
  : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // await userSessionProvider.PreloadSessions();
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
              "Ошибка при предзагрузке в кэш всех сессий при старте приложения");
        }
    }
}
