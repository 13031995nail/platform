namespace Tmobis.Reports.Stimulsoft.Service.UseCases;

using global::Stimulsoft.Base.Drawing;
using global::Stimulsoft.Report;
using global::Stimulsoft.Report.Components;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Drawing;
using Tmobis.Data.DB.MSSQL;
using Tmobis.Data.DB.Postgres;
using Tmobis.Reports.Models;
using Tmobis.Reports.Stimulsoft.Service.Options;

public interface ISqlReportBuilder : IReportBuilder
{
}

public class SqlReportBuilder(
  IConfiguration configuration,
  IOptions<ReportServiceOptions> options,
  ILogger<SqlReportBuilder> logger)
  : ISqlReportBuilder
{
    private readonly ReportServiceOptions options = options.Value;

    public ReportResult BuildReport(GetReportRequest request, IReport reportModel)
    {
        if (reportModel?.UserScript == null)
        {
            return null;
        }

        var report = new StiReport();
        var dataSet = LoadReportData(reportModel.UserScript, request.Params);

        try
        {
            FillReport(reportModel, report, dataSet);
            return new ReportResult()
            {
                Report = report,
                Context = dataSet,
            };
        }
        catch
        {
            report?.Dispose();
            dataSet?.Dispose();
            throw;
        }
    }

    private DataSet LoadReportData(
      string userScript,
      IReadOnlyDictionary<string, string>? parameters)
    {
        var dataSet = new DataSet();
        try
        {
            var connstr = configuration.GetConnectionString("Default");
            if (string.IsNullOrEmpty(connstr))
            {
                throw new InvalidOperationException(
                  "Строка подключения по умолчанию не найдена");
            }

            using var connection = CreateConnection(connstr);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = userScript;
            command.CommandTimeout = options.CommandTimeout;

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    var dbparam = command.CreateParameter();
                    dbparam.ParameterName = $"@{param.Key}";
                    dbparam.Value = (object?)(param.Value == "null" ? null : param.Value)
                        ?? DBNull.Value;
                    command.Parameters.Add(dbparam);
                }
            }

            using var adapter = CreateDataAdapter(connstr, command);
            adapter.Fill(dataSet);
            return dataSet;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при построении отчета");
            return dataSet;
        }
    }

    private static DbConnection CreateConnection(string connString)
    {
        var isPostgres = PostgresConnectionStringValidator
            .IsValidConnectionString(connString);
        if (isPostgres)
        {
            return new NpgsqlConnection(connString);
        }

        var isMssql = MSSQLConnectionStringValidator
          .IsValidConnectionString(connString);
        if (isMssql)
        {
            return new SqlConnection(connString);
        }

        throw new Exception("Не удалось определить тип подключения по строке подключения");
    }

    private static DbDataAdapter CreateDataAdapter(string connString, DbCommand command)
    {
        var isPostgres = PostgresConnectionStringValidator
            .IsValidConnectionString(connString);
        if (isPostgres)
        {
            return new NpgsqlDataAdapter((NpgsqlCommand)command);
        }

        var isMssql = MSSQLConnectionStringValidator
          .IsValidConnectionString(connString);
        if (isMssql)
        {
            return new SqlDataAdapter((SqlCommand)command);
        }

        throw new Exception("Не удалось определить тип адаптера по строке подключения");
    }

    private void FillReport(IReport reportModel, StiReport report, DataSet dataSet)
    {
        StiPage page = report.Pages[0];
        var dt = dataSet.Tables["Table"]
          ?? (dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null);
        if (dt == null)
        {
            return;
        }

        report.Dictionary.Clear();
        report.RegData(dataSet);
        report.Dictionary.Synchronize();

        // Create TitleBand
        StiHeaderBand TitleBand = new StiHeaderBand();
        TitleBand.Height = 0.85;
        TitleBand.Name = "TitleBand";
        TitleBand.PrintIfEmpty = true;
        TitleBand.PrintOnAllPages = false;
        page.Components.Add(TitleBand);

        // Create Title text on header
        StiText headerText = new StiText(new RectangleD(0, 0, page.Width, 0.85));
        headerText.Text = reportModel.Name;
        headerText.HorAlignment = StiTextHorAlignment.Left;
        headerText.Name = "TitleHeader";
        TitleBand.Components.Add(headerText);

        // Create HeaderBand
        StiHeaderBand headerBand = new StiHeaderBand();
        headerBand.Height = 0.5;
        headerBand.Name = "HeaderBand";
        headerBand.PrintOnAllPages = false;
        page.Components.Add(headerBand);

        // Create Databand
        StiDataBand dataBand = new StiDataBand();
        dataBand.DataSourceName = "Table";
        dataBand.Height = 0.5;
        dataBand.Name = "DataBand";
        page.Components.Add(dataBand);

        double pos = 0;
        page.Width = 2 * dt.Columns.Count;
        double columnWidth = 2;
        int nameIndex = 1;
        foreach (DataColumn dataColumn in dt.Columns)
        {
            // Create text on header
            StiText hText = new StiText(new RectangleD(pos, 0, columnWidth, 0.5));

            hText.Text.Value = dataColumn.Caption;
            hText.HorAlignment = StiTextHorAlignment.Center;
            hText.VertAlignment = StiVertAlignment.Center;
            hText.Name = "HeaderText" + nameIndex.ToString();
            hText.Brush = new StiSolidBrush(Color.DarkGray);
            hText.Border.Side = StiBorderSides.All;
            headerBand.Components.Add(hText);

            StiText dataText = new StiText(new RectangleD(pos, 0, columnWidth, 0.5));

            dataText.Text = "{Table." + global::Stimulsoft.Report.CodeDom.StiCodeDomSerializator.ReplaceSymbols(dataColumn.ColumnName) + "}";
            dataText.Name = "DataText" + nameIndex.ToString();
            dataText.Border.Side = StiBorderSides.All;

            dataBand.Components.Add(dataText);
            pos = pos + columnWidth;
            nameIndex++;
        }
    }
}
