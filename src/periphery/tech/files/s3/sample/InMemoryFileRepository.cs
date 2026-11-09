namespace Tmobis.Files.Adapter.S3.Sample;

using System.Collections.Concurrent;
using System.Linq.Expressions;
using Tmobis.Business.Core.Models.Files;
using Tmobis.Core.Entities;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

internal sealed class InMemoryFileRepository : IRepository, IUnitOfWork
{
  private readonly ConcurrentDictionary<Guid, IFile> storage = new();

  public Task<T?> GetById<T>(
    Guid id,
    IEntityView? view = null,
    CancellationToken cancellationToken = default)
    where T : class, IEntity
  {
    cancellationToken.ThrowIfCancellationRequested();
    return Task.FromResult(
      storage.TryGetValue(id, out var file)
        ? CastFile<T>(file)
        : null);
  }

  public Task<List<T>> GetByIds<T>(
    IReadOnlyCollection<Guid> ids,
    IEntityView? view = null,
    CancellationToken cancellationToken = default)
    where T : class, IEntity
  {
    cancellationToken.ThrowIfCancellationRequested();
    ArgumentNullException.ThrowIfNull(ids);
    return Task.FromResult(
      ids
        .Where(storage.ContainsKey)
        .Select(id => CastFile<T>(storage[id]))
        .ToList());
  }

  public Task<List<T>> Find<T>(
    Expression<Func<T, bool>>? predicate = null,
    IEntityView? view = null,
    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
    where T : class, IEntity
  {
    cancellationToken.ThrowIfCancellationRequested();
    IEnumerable<T> files = storage.Values.Select(CastFile<T>);
    if (predicate != null)
    {
      var compiled = predicate.Compile();
      files = files.Where(compiled);
    }

    if (orderBy != null)
    {
      files = orderBy(files.AsQueryable());
    }

    if (skip.HasValue)
    {
      files = files.Skip(skip.Value);
    }

    if (take.HasValue)
    {
      files = files.Take(take.Value);
    }

    return Task.FromResult(files.ToList());
  }

  public Task<T?> FirstOrDefault<T>(
    Expression<Func<T, bool>> predicate,
    IEntityView? view = null,
    CancellationToken cancellationToken = default)
    where T : class, IEntity
  {
    cancellationToken.ThrowIfCancellationRequested();
    var compiled = predicate.Compile();
    return Task.FromResult(
      storage.Values.Select(CastFile<T>).FirstOrDefault(compiled));
  }

  public Task<List<T>> All<T>(
    IEntityView? view = null,
    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
    where T : class, IEntity
    => Find<T>(null, view, orderBy, skip, take, cancellationToken);

  public Task<bool> Any<T>(
    Expression<Func<T, bool>> predicate,
    IEntityView? view = null,
    CancellationToken cancellationToken = default)
    where T : class, IEntity
  {
    cancellationToken.ThrowIfCancellationRequested();
    var compiled = predicate.Compile();
    return Task.FromResult(storage.Values.Select(CastFile<T>).Any(compiled));
  }

  public Task<int> Count<T>(
    Expression<Func<T, bool>>? predicate = null,
    IEntityView? view = null,
    CancellationToken cancellationToken = default)
    where T : class, IEntity
  {
    cancellationToken.ThrowIfCancellationRequested();
    var files = storage.Values.Select(CastFile<T>);
    if (predicate == null)
    {
      return Task.FromResult(files.Count());
    }

    var compiled = predicate.Compile();
    return Task.FromResult(files.Count(compiled));
  }

  public IQueryable<T> Query<T>(IEntityView? view = null)
    where T : class, IEntity
  => storage.Values.Select(CastFile<T>).AsQueryable();

  public T MakeEntity<T>(Guid? id = null)
    where T : class, IEntity
  => CastFile<T>(new Tmobis.Files.File
  {
    Id = id ?? Guid.NewGuid()
  });

  public Task SubLoad<T>(
    T entity,
    IEntityView view,
    CancellationToken cancellationToken = default)
    where T : class, IEntity
  {
    cancellationToken.ThrowIfCancellationRequested();
    return Task.CompletedTask;
  }

  public void Update(IEntity model)
  => Upsert(model);

  public void Update(IEnumerable<IEntity> models)
  {
    ArgumentNullException.ThrowIfNull(models);
    foreach (var model in models)
    {
      Upsert(model);
    }
  }

  public void Create(IEntity model)
  => Upsert(model);

  public void Create(IEnumerable<IEntity> models)
  => Update(models);

  public void Delete(IEntity model)
  {
    ArgumentNullException.ThrowIfNull(model);
    Delete(model.Id);
  }

  public void Delete(IEnumerable<IEntity> models)
  {
    ArgumentNullException.ThrowIfNull(models);
    foreach (var model in models)
    {
      Delete(model);
    }
  }

  public void Delete<T>(IEnumerable<T> models)
    where T : IEntity
  {
    ArgumentNullException.ThrowIfNull(models);
    foreach (var model in models)
    {
      if (model is IEntity entity)
      {
        Delete(entity);
      }
    }
  }

  public void Delete<T>(Guid id)
    where T : class, IEntity
  => Delete(id);

  public void Delete<T>(IReadOnlyCollection<Guid> ids)
    where T : class, IEntity
  {
    ArgumentNullException.ThrowIfNull(ids);
    foreach (var id in ids)
    {
      Delete(id);
    }
  }

  public Task Commit(CancellationToken cancellationToken = default)
  {
    cancellationToken.ThrowIfCancellationRequested();
    return Task.CompletedTask;
  }

  private static T CastFile<T>(IFile file)
    where T : class, IEntity
  {
    if (file is T typedFile)
    {
      return typedFile;
    }

    throw new NotSupportedException(
      $"InMemoryFileRepository поддерживает только {nameof(IFile)}.");
  }

  private void Upsert(IEntity model)
  {
    ArgumentNullException.ThrowIfNull(model);
    if (model is not IFile file)
    {
      throw new NotSupportedException(
        $"InMemoryFileRepository поддерживает только {nameof(IFile)}.");
    }

    storage[file.Id] = file;
  }

  private void Delete(Guid id)
  => storage.TryRemove(id, out _);
}
