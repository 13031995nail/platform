namespace Tmobis.Core.Repositories;

using Models;

public interface IWriteRepository
{
    public T MakeEntity<T>(Guid? id = null)
        where T : class, IEntity;

    public void Update(IEntity model);

    public void Update(IEnumerable<IEntity> models);

    public void Create(IEntity model);

    public void Create(IEnumerable<IEntity> models);

    public void Delete(IEntity model);

    public void Delete(IEnumerable<IEntity> models);

    public void Delete<T>(IEnumerable<T> models)
        where T : IEntity;

    public void Delete<T>(Guid id)
        where T : class, IEntity;

    public void Delete<T>(IReadOnlyCollection<Guid> ids)
        where T : class, IEntity;
}
