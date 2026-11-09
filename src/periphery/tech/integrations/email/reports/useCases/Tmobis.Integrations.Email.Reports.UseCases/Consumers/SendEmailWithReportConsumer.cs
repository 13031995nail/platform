namespace Tmobis.Integrations.Email.Reports.UseCases.Consumers;

using System;
using System.Collections.Generic;
using System.Text;
using Core.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Producers;
using Tmobis.Files.Provider;
using Tmobis.Files.Uploader;
using Tmobis.Integrations.Email.Consumer.Consumers;
using Tmobis.Integrations.Email.Reports;
using Tmobis.Reports.Adapter.SecondaryAdapters;
using Tmobis.Reports.Adapter.UseCases;

public class SendEmailWithReportConsumer(
    IFileUploader fileUploader,
    IMessageDispatcher messageDispatcher,
    ILogger<SendEmailWithReportConsumer> logger,
    IReportServiceAdapter reportAdapter
) : IMessageConsumer<SendEmailWithReport, SendEmailResult>
{
    public async Task<SendEmailResult> ConsumeWithResult(SendEmailWithReport message)
    {
        try
        {
            if (message.ReportInfo == null)
                return new SendEmailResult { Error = false };
            
            var fileName = message.ReportInfo.FileName;
            var request = message.ReportInfo.ReportRequest;
            var report = await reportAdapter.BuildReport(request);
            var fileId = await fileUploader.UploadFile(report, fileName);
            
            var newFileIds = message.FileIds ?? [];
            newFileIds.Add(new SendEmailFile { FileId = fileId, FileName = fileName });
            var emailMessage = new SendEmail
            {
                Addresses = message.Addresses,
                Subject = message.Subject,
                Body = message.Body,
                HighPriority = message.HighPriority,
                DiffLetters = message.DiffLetters,
                FileIds = newFileIds
            };
            await messageDispatcher.Send(emailMessage);
            return new SendEmailResult { Error = false };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, @"Внутренняя ошибка обработки");
            return new SendEmailResult { Error = true };
        }
    }
}
