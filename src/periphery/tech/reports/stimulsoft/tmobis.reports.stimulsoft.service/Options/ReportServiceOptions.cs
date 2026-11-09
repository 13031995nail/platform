namespace Tmobis.Reports.Stimulsoft.Service.Options;

public class ReportServiceOptions
{
  public const string ReportService = "ReportService";

  public string ReportsPath { get; set; }

  public string FontsPath { get; set; }

  public string StimulsoftLicenseKey { get; set; }

  public int MassParallelLines { get; set; } = 4;

  public int CommandTimeout { get; set; } = 3000;
}
