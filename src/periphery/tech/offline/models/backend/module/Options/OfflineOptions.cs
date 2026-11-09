namespace Tmobis.Offline.Options;

public class OfflineOptions
{
    public const string Offline = "Offline";

    public string OfflineBackupDirectory { get; set; } = string.Empty;

    public string OfflineBackupFileName { get; set; }
    = $"{nameof(Models.JsonBackupInfo)}.json";
}