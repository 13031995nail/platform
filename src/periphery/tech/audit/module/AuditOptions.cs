namespace Tmobis.Audit;

public class AuditOptions
{
    public const string Audit = "Audit";

    public bool IsEnabled { get; set; } = true;

    public bool IsEnabledRead { get; set; } = false;
}
