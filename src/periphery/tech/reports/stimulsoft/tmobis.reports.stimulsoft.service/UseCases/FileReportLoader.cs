namespace Tmobis.Reports.Stimulsoft.Service.UseCases;
using global::Stimulsoft.Report;
using Microsoft.Extensions.Options;
using Tmobis.Reports.Stimulsoft.Service.Options;

public interface IFileReportLoader : IReportLoader
{
}

public class FileReportLoader(
  IOptions<ReportServiceOptions> options, IConfiguration configuration)
: IFileReportLoader
{
  private readonly ReportServiceOptions options = options.Value;
  private readonly string? contentPath = configuration?.GetValue<string>(WebHostDefaults.ContentRootKey);

  public StiReport? LoadReport(string reportName)
  {
    if (string.IsNullOrEmpty(contentPath))
    {
      throw new Exception($"Не удалось определить {nameof(contentPath)}");
    }

    var reportPath = Path.Combine(contentPath, options.ReportsPath, Path.GetFileNameWithoutExtension(reportName));
    var report = TryLoadDllReport(reportPath);
    report ??= TryLoadMrtReport(reportPath);

    return report;
  }

  private StiReport? TryLoadDllReport(string reportPath)
  {
    var dllPath = $"{reportPath}.dll";
    if (!File.Exists(dllPath))
    {
      return null;
    }

    return StiReport.GetReportFromAssembly(dllPath, false);
  }

  private StiReport? TryLoadMrtReport(string reportPath)
  {
    var mrtPath = $"{reportPath}.mrt";
    if (!File.Exists(mrtPath))
    {
      return null;
    }

    using var fileStream = new MemoryStream(File.ReadAllBytes(mrtPath));
    var report = new StiReport();
    report.Load(fileStream);
    return report;
  }
}