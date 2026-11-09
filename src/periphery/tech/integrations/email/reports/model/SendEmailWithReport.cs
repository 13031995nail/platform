namespace Tmobis.Integrations.Email.Reports;

using Tmobis.Core.Medium.Models;
using Tmobis.Integrations.Email;

[MessageMetadata(Group = "email")]
public record SendEmailWithReport : SendEmail
{
    public SendEmailReport? ReportInfo { get; init; } = null;
}
