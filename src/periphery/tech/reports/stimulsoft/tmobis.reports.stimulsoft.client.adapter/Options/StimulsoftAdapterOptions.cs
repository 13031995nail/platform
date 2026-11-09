namespace Tmobis.Reports.Stimulsoft.Client.Adapter.Options;

public class StimulsoftAdapterOptions
{
  public const string Stimulsoft = "Stimulsoft";

  public string ApiKey { get; set; }

  public string ServiceUrl { get; set; }

  public string BuildReportPath { get; set; }

  public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(2);
}
