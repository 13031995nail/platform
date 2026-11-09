namespace Tmobis.Core.Repositories;

public interface IUnitOfWork
{
    public Task Commit(CancellationToken cancellationToken = default);
}
