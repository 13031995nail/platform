namespace Tmobis.Reports.Stimulsoft.Service.UseCases;

using global::Stimulsoft.Report;
using global::Stimulsoft.Report.Export;
using ICSSoft.STORMNET.Business;
using ICSSoft.STORMNET.Business.LINQProvider;
using Microsoft.Extensions.Options;
using Tmobis.Reports.Models;
using Tmobis.Reports.Stimulsoft.Service.Options;

public interface IReportExporter
{
  public byte[] ExportReport(GetReportRequest request);
}

public class ReportExporter(
  IFileReportBuilder fileBuilder,
  ISqlReportBuilder sqlBuilder,
  IDataService dataService,
  IOptions<ReportServiceOptions> options) : IReportExporter
{
  private readonly IDataService dataService = dataService;
  private readonly ReportServiceOptions options = options.Value;

  private StiExportFormat MapExportFormat(ReportFormat? exportFormat)
  => exportFormat switch
  {
    ReportFormat.Html => StiExportFormat.Html,
    ReportFormat.Docx => StiExportFormat.Word,
    ReportFormat.Pdf => StiExportFormat.Pdf,
    ReportFormat.Xlsx => StiExportFormat.Excel,
    ReportFormat.Csv => StiExportFormat.Csv,
    _ => throw new NotSupportedException("Неподдерживаемый формат экспорта"),
  };

  public byte[]? ExportReport(GetReportRequest request)
  {
    ReportResult? reportResult = null;
    try
    {
      var reportName = request.TemplateName;
      var parameters = request.Params;
      var exportFormat = request.Format;
      var reportModel = LoadModel(reportName);

      reportResult = sqlBuilder.BuildReport(request, reportModel);
      if (reportResult?.Report == null)
      {
        reportResult = fileBuilder.BuildReport(request, reportModel);
      }

      RenderReport(reportResult.Report);
      var exportedReport = ExportReport(reportModel, reportResult.Report, exportFormat);
      return exportedReport;
    }
    finally
    {
      reportResult?.Report?.Dispose();
      reportResult?.Context?.Dispose();
      reportResult = null;
      GC.Collect();
    }
  }

  private Flexberry.Models.Report? LoadModel(string reportName)
  {
    // TODO: Может быть null, т.к. приклад посылает reportName уже с учетом SubType
    var reportNameAdd = $"{reportName}?";
    return dataService
      .Query<Flexberry.Models.Report>(Flexberry.Models.Report.Views.ReportE)
      .FirstOrDefault(report => report.VirtualName == reportName
        || report.VirtualName.StartsWith(reportNameAdd));
  }

  private void RenderReport(StiReport report)
  {
    report?.ResetRenderedState();
    report?.Render();
  }

  private byte[] ExportReport(
    Flexberry.Models.Report? reportModel,
    StiReport? report,
    ReportFormat? exportFormat)
  {
    var protectExcel = false;
    if (reportModel != null)
    {
      var reportNameSplit = reportModel.VirtualName.Split("?");
      var reportParamsStr = reportNameSplit.Length > 1
        ? reportNameSplit[1]
        : null;
      var reportParams = reportParamsStr?.Split("&");
      protectExcel = reportParams?.Any(x => x.StartsWith("Protected")) ?? false;
    }

    using var stream = new MemoryStream();
    StiExportSettings? settings = null;

    var stiExportFormat = MapExportFormat(exportFormat);
    switch (stiExportFormat)
    {
      case StiExportFormat.Csv:
        settings = new StiCsvExportSettings
        {
          DataExportMode = StiDataExportMode.DataAndHeadersFooters,
          SkipColumnHeaders = true,
        };
        break;
      case StiExportFormat.Excel:
        StiOptions.Export.Excel.DivideBigCells = false;
        settings = new StiExcel2007ExportSettings()
        {
          RestrictEditing = protectExcel
            ? StiExcel2007RestrictEditing.ExceptEditableFields
            : StiExcel2007RestrictEditing.No,
          ExportEachPageToSheet = true,
        };
        break;
    }

    report?.ExportDocument(stiExportFormat, stream, settings);
    return stream.ToArray();
  }
}
