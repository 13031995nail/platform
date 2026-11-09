namespace Tmobis.Reports.Stimulsoft.Service.UseCases;

using System.Collections.Concurrent;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tmobis.Files.Provider;
using Tmobis.Reports.Models;
using Tmobis.Reports.Stimulsoft.Service.Options;

public interface IMassReportExporter
{
  public Task<byte[]> ExportMassReport(GetMassReportRequest message);
}

public class MassReportExporter(
  IServiceProvider serviceProvider,
  IOptions<ReportServiceOptions> options,
  IFileProvider remoteFileProvider) : IMassReportExporter
{
  private readonly ReportServiceOptions options = options.Value;

  public async Task<byte[]?> ExportMassReport(GetMassReportRequest message)
  {
    var reportParams = message.ReportParams;
    var files = new ConcurrentDictionary<string, byte[]>();
    await Parallel.ForEachAsync(
    reportParams,
    new ParallelOptions()
    {
      MaxDegreeOfParallelism = options.MassParallelLines,
    },
    async (param, CancellationToken) =>
    {
      using var scope = serviceProvider.CreateScope();
      var exporter = scope.ServiceProvider.GetRequiredService<IReportExporter>();
      if (param.Params == null && param.ParamsList != null)
      {
        param.Params = new Dictionary<string, string>();
        foreach (var par in param.ParamsList)
        {
          param.Params.Add(par.Key, par.Value);
        }
      }

      byte[] reportContent = [];
      var needToExportReport = true;
      if (param.PrintingFileId != null)
      {
        try
        {
          if (param.Params.TryGetValue("actNumber", out string actNumber))
          {
            reportContent = await remoteFileProvider.GetFile((Guid)param.PrintingFileId, actNumber + ".pdf");
            needToExportReport = false;
          }
        }
        catch { }
      }
      if (needToExportReport)
      {
        reportContent = exporter.ExportReport(new GetReportRequest()
        {
          Format = ReportFormat.Pdf,
          Params = param.Params,
          TemplateName = param.TemplateName,
          FilialId = param.TownId,
        });
      }

      files.TryAdd(param.FileName.Replace("/", "_") + ".pdf", reportContent);
    });

    using var compressedFileStream = new MemoryStream();
    using (ZipOutputStream outputStream = new ZipOutputStream(compressedFileStream))
    {
      outputStream.SetLevel(0);
      foreach (var param in files)
      {
        ZipEntry entry = new ZipEntry(param.Key);
        entry.IsUnicodeText = true;
        await outputStream.PutNextEntryAsync(entry);
        var bytes = param.Value;
        await outputStream.WriteAsync(bytes, 0, bytes.Length);
      }

      outputStream.Finish();
      outputStream.Close();
    }

    return compressedFileStream.ToArray();
  }
}
