namespace Tmobis.Offline.Backups.Messages;
using Tmobis.Core.Medium.Models;

[MessageMetadata(Group = "offline")]
public record MakeOfflineBackup : DomainEvent;
