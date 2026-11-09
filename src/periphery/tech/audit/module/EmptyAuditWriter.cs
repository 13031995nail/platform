namespace Tmobis.Audit;

/// <summary>
/// Заглушка сервиса записи аудита
/// </summary>
public class EmptyAuditWriter
: IAuditWriter
{
    /// <inheritdoc/>
    public Task AddEvent(AuditRequest request)
    => Task.CompletedTask;
}
