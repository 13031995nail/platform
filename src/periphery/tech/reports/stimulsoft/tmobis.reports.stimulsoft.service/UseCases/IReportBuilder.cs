namespace Tmobis.Reports.Stimulsoft.Service.UseCases;

using global::Stimulsoft.Report;
using Tmobis.Reports.Models;

public interface IReportBuilder
{
  public ReportResult BuildReport(GetReportRequest request, IReport reportModel);
}

public record ReportResult
{
    public StiReport? Report { get; init; }

    public IDisposable? Context { get; init; }
}