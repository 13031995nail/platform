using System.Net;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewPlatform.Flexberry.ORM.ODataService.Batch;

namespace Tmobis.Frameworks.Flexberry.Web.OData.Batch;

//
// Summary:
//     Batch handler for DataService.
public class FlexberryODataBatchHandler(
    ILogger<FlexberryODataBatchHandler> logger)
 : DataObjectODataBatchHandler
{
    private readonly ILogger<FlexberryODataBatchHandler> logger = logger;

    public override async Task ProcessBatchAsync(HttpContext context, RequestDelegate nextHandler)
    {
        string body = null;
        try
        {
            var odataOptions = context.RequestServices.GetRequiredService<ODataOptions>();
            if (odataOptions != null)
            {
                odataOptions.EnableContinueOnErrorHeader = true;
            }
            context.Request.EnableBuffering();
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
              body = await reader.ReadToEndAsync();
            }
            context.Request.Body.Position = 0;
            await base.ProcessBatchAsync(context, nextHandler);
        }
        catch (Exception ex)
        {
            // IList<ODataBatchRequestItem> subRequests = await ParseBatchRequestsAsync(context);
            // logger.LogError(ex, "Ошибка при обработке батча: {batch}", body);
            Task.Run(async () => await LogToFileAsync(body, ex));
            throw;
        }
    }

    private async Task LogToFileAsync(string body,Exception ex)
    {
      try
      {
        string logContent = $"{DateTime.Now:HH.mm.ss dd-MM-yyyy}\nBodyBatch:\n{body}\nError:\n{ex}";
        string fileName = $"batchlog-{DateTime.Now:HH.mm.ss dd-MM-yyyy}.txt";
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BatchLogs", fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        await File.AppendAllTextAsync(filePath, logContent);
      }
      catch (Exception exception)
      {
        logger.LogError(exception, "Ошибка при записи batch-лога на диск");
      }
    }
}
