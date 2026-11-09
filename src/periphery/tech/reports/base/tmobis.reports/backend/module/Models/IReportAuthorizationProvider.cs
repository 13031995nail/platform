namespace Tmobis.Reports.Models;

public interface IReportAuthorizationProvider
{
    bool Supports(ReportRunParams runParams, string? subTypeId, string? filialId);

    Task EnsureCanRun(ReportRunParams runParams, string? subTypeId, string? filialId);
}
