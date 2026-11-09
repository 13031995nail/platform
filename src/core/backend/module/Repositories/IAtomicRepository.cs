namespace Tmobis.Core.Repositories;

/// <summary>
/// Сохранение накопленных изменений с обязательным откатом при ошибке любого запроса.
/// </summary>
public interface IAtomicRepository : IRepository, IUnitOfWork
{
    Task CommitAtomically();
}
