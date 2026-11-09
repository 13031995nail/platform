namespace Tmobis.Reports.Adapter.UseCases;

using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tmobis.Core.Medium.Producers;
using Tmobis.Reports.Adapter.SecondaryAdapters;
using Tmobis.Reports.Models;

/// <summary>
/// Сценарии использования сервиса ПФ.
/// </summary>
public class ReportUseCase(
    IReportServiceAdapter reportAdapter,
    IMessageDispatcher messageDispatcher,
    IEnumerable<IReportAuthorizationProvider> authorizationProviders
) : IReportUseCase
{
    public virtual async Task<byte[]> GetFile(
        ReportRunParams runParams,
        string? subTypeId = null,
        string? filialId = null,
        bool throwOnError = false)
    {
        foreach (var provider in authorizationProviders)
        {
            if (!provider.Supports(runParams, subTypeId, filialId))
            {
                continue;
            }

            await provider.EnsureCanRun(runParams, subTypeId, filialId);
        }

        var request = new GetReportRequest()
        {
            TemplateName = runParams.TemplateName,
            Params = runParams.Params,
            Format = MapReportFormat(runParams.Format),
            SubType = subTypeId,
            FilialId = filialId,
        };

        return await reportAdapter.BuildReport(request, throwOnError);
    }

    public async Task GetBigFile(GetBigReportRequest request)
    {
        await messageDispatcher.Send(request);
    }

    public virtual async Task GetMassPdf(GetMassReportRequest request)
    {
        await messageDispatcher.Send(request);
    }

    public virtual ReportFormat MapReportFormat(string format)
    {
        switch (format)
        {
            case "word":
            case "word2007":
                return ReportFormat.Docx;
            case "excel":
            case "excel2007":
                return ReportFormat.Xlsx;
            case "excelbig":
                return ReportFormat.Csv;
            case "pdf":
                return ReportFormat.Pdf;
            case "html":
                return ReportFormat.Html;
            default:
                throw new Exception("Неподдерживаемый формат");
        }
    }

    public virtual string MapFileFormat(string format)
    {
        switch (format)
        {
            case "word":
            case "word2007":
                return "docx";
            case "excel":
            case "excel2007":
                return "xlsx";
            case "excelbig":
                return "csv";
            case "pdf":
                return "pdf";
            case "html":
                return "html";
            default:
                throw new Exception("Неподдерживаемый формат");
        }
    }
}
