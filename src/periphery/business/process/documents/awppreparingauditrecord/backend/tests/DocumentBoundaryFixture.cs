namespace Tmobis.Business.Process.Documents.Tests;

using System.Linq.Expressions;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Core.Flexberry.Models;
using Tmobis.Business.Core.Models.Employees;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Business.Core.Providers.Users;
using Tmobis.Core.Entities;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;
using Employee = Tmobis.Business.Core.Flexberry.Models.Employee;
using User = Tmobis.Business.Core.Flexberry.Models.User;

internal sealed class DocumentBoundaryFixture
{
    private readonly List<IEntity> entities = [];
    public IRepository Repository { get; }
    public TestWorkObjectAccessProvider Access { get; } = new();
    public BoundaryRoles Roles { get; } = new();
    public BoundaryUsers Users { get; } = new();
    public Awp Awp { get; } = new();
    public Organization Municipality { get; } = new();
    public WorkObject Object { get; }
    public int Commits { get; private set; }

    public DocumentBoundaryFixture()
    {
        Access.CurrentEmployeeId = Users.Employee.Id;
        Add(Awp);
        Object = Add(new WorkObject
        {
            Organization = new Organization { Hierarchy = Municipality }, Actuality = true,
        });
        Repository = new InMemoryDocumentRepository(entities, onCommit: () => Commits++);
    }

    public T Add<T>(T entity) where T : class, IEntity
    {
        entities.Add(entity);
        return entity;
    }

    public void Allow(params WorkObject[] objects)
        => ((HashSet<Guid>)Access.GenericIds).UnionWith(objects.Select(item => item.Id));

    public void AllowForRole(string role, params WorkObject[] objects)
    {
        Access.RoleIds[role] = objects.Select(item => item.Id).ToHashSet();
        Allow(objects);
    }

}

internal sealed class InMemoryDocumentRepository(
        List<IEntity> entities,
        Action? onCommit = null,
        Action<List<IEntity>>? onUpdate = null,
        Action? onAtomicCommit = null,
        Action<Type>? onRead = null) : IAtomicRepository
    {
        public Task<T?> GetById<T>(Guid id, IEntityView? view = null, CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            onRead?.Invoke(typeof(T));
            return Task.FromResult(entities.OfType<T>().FirstOrDefault(item => item.Id == id));
        }

        public Task<List<T>> GetByIds<T>(IReadOnlyCollection<Guid> ids, IEntityView? view = null, CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            onRead?.Invoke(typeof(T));
            return Task.FromResult(entities.OfType<T>().Where(item => ids.Contains(item.Id)).ToList());
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
            onRead?.Invoke(typeof(T));
            IQueryable<T> query = entities.OfType<T>().AsQueryable();
            if (predicate != null) query = query.Where(predicate);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);
            return Task.FromResult(query.ToList());
        }

        public Task<T?> FirstOrDefault<T>(
            Expression<Func<T, bool>> predicate,
            IEntityView? view = null,
            CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            cancellationToken.ThrowIfCancellationRequested();
            onRead?.Invoke(typeof(T));
            return Task.FromResult(entities.OfType<T>().FirstOrDefault(predicate.Compile()));
        }

        public Task<List<T>> All<T>(
            IEntityView? view = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int? skip = null,
            int? take = null,
            CancellationToken cancellationToken = default)
            where T : class, IEntity
            => Find<T>(null, view, orderBy, skip, take, cancellationToken);

        public Task<bool> Any<T>(Expression<Func<T, bool>> predicate, IEntityView? view = null, CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            onRead?.Invoke(typeof(T));
            return Task.FromResult(entities.OfType<T>().Any(predicate.Compile()));
        }

        public Task<int> Count<T>(Expression<Func<T, bool>>? predicate = null, IEntityView? view = null, CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            onRead?.Invoke(typeof(T));
            var query = entities.OfType<T>();
            return Task.FromResult(predicate == null ? query.Count() : query.Count(predicate.Compile()));
        }

        public IQueryable<T> Query<T>(IEntityView? view = null)
            where T : class, IEntity
        {
            onRead?.Invoke(typeof(T));
            return entities.OfType<T>().AsQueryable();
        }

        public T MakeEntity<T>(Guid? id = null)
            where T : class, IEntity
        {
            var entity = Activator.CreateInstance<T>()
                ?? throw new InvalidOperationException($"Cannot create {typeof(T).Name}.");
            if (id.HasValue)
            {
                entity.Id = id.Value;
            }
            return entity;
        }

        public Task SubLoad<T>(T entity, IEntityView view, CancellationToken cancellationToken = default)
            where T : class, IEntity
            => Task.CompletedTask;

        public void Update(IEntity model)
        {
            var index = entities.FindIndex(item => item.GetType() == model.GetType() && item.Id == model.Id);
            if (index < 0)
            {
                entities.Add(model);
            }
            else
            {
                entities[index] = model;
            }
        }

        public void Update(IEnumerable<IEntity> models)
        {
            if (onUpdate != null)
            {
                onUpdate(models.ToList());
                return;
            }
            foreach (var model in models)
            {
                Update(model);
            }
        }

        public void Create(IEntity model)
            => entities.Add(model);

        public void Create(IEnumerable<IEntity> models)
            => entities.AddRange(models);

        public void Delete(IEntity model)
            => entities.RemoveAll(item => item.GetType() == model.GetType() && item.Id == model.Id);

        public void Delete(IEnumerable<IEntity> models)
        {
            foreach (var model in models)
            {
                Delete(model);
            }
        }

        public void Delete<T>(IEnumerable<T> models) where T : IEntity
        {
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
            => entities.RemoveAll(item => item is T && item.Id == id);

        public void Delete<T>(IReadOnlyCollection<Guid> ids)
            where T : class, IEntity
            => entities.RemoveAll(item => item is T && ids.Contains(item.Id));

        public Task Commit(CancellationToken cancellationToken = default)
        {
            onCommit?.Invoke();
            return Task.CompletedTask;
        }

        public Task CommitAtomically()
        {
            (onAtomicCommit ?? onCommit)?.Invoke();
            return Task.CompletedTask;
        }

        private static IEnumerable<T> Page<T>(IEnumerable<T> query, int? page, int? perPage)
        {
            if (!page.HasValue || !perPage.HasValue)
            {
                return query;
            }

            var take = Math.Max(perPage.Value, 0);
            var skip = Math.Max(page.Value - 1, 0) * take;
            return query.Skip(skip).Take(take);
        }
}

internal sealed class BoundaryRoles : ICurrentRoleProvider
{
    public List<string> Names { get; set; } = [];
    public ValueTask<List<string>> GetCurrentRoleNames() => ValueTask.FromResult(Names);
}

internal sealed class BoundaryUsers : ICurrentUserProvider
{
    public Employee Employee { get; } = new();
    public Task<IUser?> GetCurrentUser() => Task.FromResult<IUser?>(new User());
    public Task<IEmployee?> GetCurrentEmployee() => Task.FromResult<IEmployee?>(Employee);
    public string? GetUserConnectionInfo() => null;
    public string? GetUserConnectionAddress() => null;
}
