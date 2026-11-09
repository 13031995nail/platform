namespace Tmobis.Core.Repositories;

using Microsoft.Extensions.DependencyInjection;

public record RepositoryScope(
    IRepository Repository,
    IUnitOfWork UnitOfWork,
    IServiceScope Scope)
    : IDisposable
{
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Scope.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Deconstruct(
        out IRepository repository,
        out IUnitOfWork unitOfWork,
        out IServiceScope scope)
    {
        repository = Repository;
        unitOfWork = UnitOfWork;
        scope = Scope;
    }
}
