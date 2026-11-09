namespace Tmobis.Integrations.Email.Reports;

using Tmobis.Reports.Models;

public record SendEmailReport
{
    public required string FileName { get; init; }
    public required GetReportRequest ReportRequest { get; init; }
}
