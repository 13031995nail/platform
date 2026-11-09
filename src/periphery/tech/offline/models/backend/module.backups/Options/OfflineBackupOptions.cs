namespace Tmobis.Offline.Backups.Options
{
  using Tmobis.Offline.Backups.Models;

  public class OfflineBackupOptions
  {
    public const string OfflineBackup = "OfflineBackup";

    public string DirectoryPath { get; set; } = string.Empty;

    public List<OfflineBackupEntity> EntityList { get; set; } = [];
  }
}
