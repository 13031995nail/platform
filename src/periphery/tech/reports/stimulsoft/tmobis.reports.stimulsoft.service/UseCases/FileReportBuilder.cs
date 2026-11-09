namespace Tmobis.Reports.Stimulsoft.Service.UseCases;

using System.Globalization;
using global::Stimulsoft.Report;
using global::Stimulsoft.Report.Dictionary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Tmobis.Core.DI;
using Tmobis.Files.Provider.Remote;
using Tmobis.Reports.Models;

public interface IFileReportBuilder : IReportBuilder
{
}

public class FileReportBuilder(
    IFileReportLoader loader,
    IConfiguration configuraton,
    IOptions<RemoteFileProviderOptions> fileOptions)
    : IFileReportBuilder
{
    private readonly RemoteFileProviderOptions fileOptions = fileOptions.Value;

    public ReportResult BuildReport(GetReportRequest request, IReport reportModel)
    {
        ArgumentNullException.ThrowIfNull(request);
        StiReport? report = null;
        try
        {
            var parameters = request.Params;
            var templateSub = $"{request.TemplateName}{request.SubType}";
            if (!string.IsNullOrEmpty(request.FilialId))
            {
                var templateFull = $"{templateSub}_{request.FilialId}";
                report = loader.LoadReport(templateFull);
            }

            report ??= loader.LoadReport(templateSub);

            if (report == null)
            {
                throw new FileNotFoundException(
                    "Отчет с указанным наименованием не найден");
            }

            FillReport(report, parameters);
            return new ReportResult()
            {
                Report = report,
            };
        }
        catch
        {
            report?.Dispose();
            throw;
        }
    }

    private void FillReport(StiReport? report,
        Dictionary<string, string>? parameters)
    {
        FillDatabases(report);
        FillParameters(report, parameters);
    }

    private void FillDatabases(StiReport? report)
    {
        if (report.Dictionary.Databases["Соединение"]
            is StiSqlDatabase stiSqlDatabase)
        {
            stiSqlDatabase.ConnectionString = configuraton
                .GetDefaultConnectionString();
        }
    }

    private void FillParameters(StiReport? report,
        Dictionary<string, string>? parameters)
    {
        parameters?.Add("filePath", fileOptions.Url);
        parameters?.Add("fileApiKey", fileOptions.ApiKey);
        parameters?.Add("fileApiKeyHeader", fileOptions.ApiKeyHeaderName);
        foreach (StiVariable stiVariable in report.Dictionary.Variables)
        {
            string? value = null;
            parameters?.TryGetValue(stiVariable.Name, out value);

            if (!string.IsNullOrWhiteSpace(value)
                || stiVariable.Name.Equals(
                    "pk", StringComparison.InvariantCulture))
            {
                stiVariable.InitBy = StiVariableInitBy.Value;
                if (value == "null")
                {
                    stiVariable.Value = null;
                    report[stiVariable.Name] = null;
                }
                else
                {
                    if (stiVariable.Type == typeof(DateTime)
                      || stiVariable.Type == typeof(DateTime?))
                    {
                        if (!DateTime.TryParseExact(
                          value,
                          "yyyy-MM-dd",
                          CultureInfo.InvariantCulture,
                          DateTimeStyles.None,
                          out DateTime result))
                        {
                            throw new Exception(
                                $"Неверное значение параметра \"{stiVariable.Name}\"");
                        }

                        report[stiVariable.Name] = result;
                        value = result.ToString(
                            "MM.dd.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    else if (stiVariable.Type == typeof(Guid)
                        || stiVariable.Type == typeof(Guid?))
                    {
                        if (!Guid.TryParse(value, out Guid result))
                        {
                            throw new Exception(
                                $"Неверное значение параметра \"{stiVariable.Name}\"");
                        }

                        report[stiVariable.Name] = result;
                    }
                    else if (stiVariable.Type == typeof(bool)
                        || stiVariable.Type == typeof(bool?))
                    {
                        if (!bool.TryParse(value, out bool result))
                        {
                            throw new Exception(
                                $"Неверное значение параметра \"{stiVariable.Name}\"");
                        }

                        report[stiVariable.Name] = result;
                    }
                    else
                    {
                        report[stiVariable.Name] = value;
                    }

                    stiVariable.Value = value;
                }
            }
        }
    }
}
