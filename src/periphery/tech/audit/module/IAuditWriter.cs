namespace Tmobis.Audit;

/// <summary>
/// Аудит операций над сущностями
/// </summary>
public interface IAuditWriter
{
    /// <summary>
    /// Регистрирует событие аудита по сущности
    /// </summary>
    /// <param name="request">Данные операции с сущностью</param>
    Task AddEvent(AuditRequest request);
}
