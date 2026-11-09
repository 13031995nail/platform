namespace Tmobis.Reports.Stimulsoft.Service.UseCases;

using global::Stimulsoft.Report;

public interface IReportLoader
{
  public StiReport? LoadReport(string reportName);
}