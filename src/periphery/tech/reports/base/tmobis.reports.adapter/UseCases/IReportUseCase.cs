namespace Tmobis.Reports.Adapter.UseCases;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tmobis.Reports.Models;

public interface IReportUseCase
{
    /// <summary>
    /// Синхронный (с ожиданием ответа) запрос одной ПФ.
    /// </summary>
    public Task<byte[]> GetFile(
        ReportRunParams runParams,
        string? subTypeId = null,
        string? filialId = null,
        bool throwOnError = false);
        
    /// <summary>
    /// Асинхронный (без ожидания ответа) запрос одной ПФ.
    /// </summary>
    public Task GetBigFile(GetBigReportRequest request);

    /// <summary>
    /// Асинхронный (без ожидания ответа) запрос списка ПФ.
    /// </summary>
    public Task GetMassPdf(GetMassReportRequest request);

    public ReportFormat MapReportFormat(string format);

    public string MapFileFormat(string format);
}
