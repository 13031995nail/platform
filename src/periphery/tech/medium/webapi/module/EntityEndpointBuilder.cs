namespace Tmobis.Medium.WebApi;

using Microsoft.AspNetCore.Routing;
using Core.Entities;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Entities.Filters;
using Core.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Core.Repositories;
using Core.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Audit;

public class EntityEndpointBuilder(
    IEntityRegister entityRegister,
    IEntityMaterializer materializer,
    IOptions<EntityWebApiOptions> options,
    IEntitySearchExpressionBuilder searchExpressionBuilder,
    IEntityFilterExpressionBuilder filterExpressionBuilder,
    ISortingExpressionBuilder sortingExpressionBuilder)
: ApiEndpointBuilder
{
    private readonly IReadOnlyCollection<Type> entities
    = entityRegister.EntityInterfaces;
    private readonly EntityWebApiOptions options = options.Value;

    public override void Build(IEndpointRouteBuilder endpoints)
    {
        foreach (var entityIface in entities)
        {
            var entityType = materializer
                .ResolveBusinessEntityType(entityIface);
            var genericMethod = GetType()
                .GetMethod(nameof(ConfigureEntityEndpoints),
                    BindingFlags.Instance | BindingFlags.NonPublic)?
                .MakeGenericMethod(entityIface, entityType);
            genericMethod?.Invoke(this, [endpoints]);
        }
    }

    protected virtual void ConfigureEntityEndpoints<TI, T>(
        IEndpointRouteBuilder endpoints)
    where TI: class, IEntity
    where T: class, TI
    {
        JsonSerializerSettings updateSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new IgnoreEntityLinksResolver(),
            Converters = [ new EntityCreationConverter(materializer) ]
        };

        var selectSettings = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        { ReferenceHandler = ReferenceHandler.IgnoreCycles };

        var ifaceType = typeof(TI);
        var entityType = typeof(T);
        var entityName = entityType.Name;

        var entityGroup = endpoints.MapGroup(BuildEndpoint(entityName))
            .WithTags(entityName)
            .RequireAuthorization();
        var internalEntityMarker = entityType.GetCustomAttribute<InternalEntityAttribute>()
                                   ?? ifaceType.GetCustomAttribute<InternalEntityAttribute>();

        if (IsMethodAllowed(internalEntityMarker, EntityVisibility.Read))
        {
            ConfigureGetByIdEndpoint<T>(entityGroup, selectSettings);
            ConfigureListEndpoint<T>(entityGroup, selectSettings);
        }

        if (IsMethodAllowed(internalEntityMarker, EntityVisibility.Create))
        {
            ConfigureCreateEndpoint<T>(entityGroup, updateSettings, selectSettings);
        }

        if (IsMethodAllowed(internalEntityMarker, EntityVisibility.Update))
        {
            ConfigureUpdateEndpoint<T>(entityGroup, updateSettings, selectSettings);
        }

        if (IsMethodAllowed(internalEntityMarker, EntityVisibility.Delete))
        {
            ConfigureDeleteEndpoint<T>(entityGroup);
            ConfigureDeleteListEndpoint<T>(entityGroup);
        }
    }

    private static void ConfigureGetByIdEndpoint<TI>(
        RouteGroupBuilder entityGroup,
        JsonSerializerOptions selectSettings)
    where TI: class, IEntity
    {
        entityGroup.MapGet("{id:guid}",
        async (Guid id,
            [FromQuery] string? viewName,
            [FromServices] IEntityMaterializer materializer,
            [FromServices] IAuditWriter auditWriter,
            [FromKeyedServices(RepositoryKeys.TownLimited)] IRepository repository,
            [FromKeyedServices(RepositoryKeys.TownLimited)] IUnitOfWork unitOfWork) =>
        {
            var entity = await repository.GetById<TI>(
                id, materializer.GetViewName<TI>(viewName));

            if (entity == null)
            {
                return Results.NotFound();
            }

            // TODO: Сделать событием через сообщения
            await auditWriter.AddEvent(new AuditRequest
            {
                Entity = entity,
                EventType = AuditEventType.Read
            });

            await unitOfWork.Commit();
            return Results.Json(entity, selectSettings);
        });
    }



    protected virtual (Expression<Func<TI, bool>>? Predicate, IEntityView? View, IResult? ErrorResult) PrepareQueryPipeline<TI>(
    string[]? filter,
    string? search,
    string? viewName,
    IFilterParser filterParser,
    IEntityMaterializer materializer)
where TI : class, IEntity
    {
        var filterResult = ParseFilters(filterParser, filter);
        if (filterResult.Error != null)
        {
            return (null, null, filterResult.Error);
        }

        var view = materializer.GetViewName<TI>(viewName);

        var predicateResult = BuildPredicate<TI>(search, filterResult.Filters, view);
        if (predicateResult.Error != null)
        {
            return (null, null, predicateResult.Error);
        }

        return (predicateResult.Predicate, view, null);
    }

    private void ConfigureListEndpoint<TI>(
    RouteGroupBuilder entityGroup,
    JsonSerializerOptions selectSettings)
where TI: class, IEntity
    {
        entityGroup.MapGet("",
        async ([FromQuery] int? perPage,
        [FromQuery] string? viewName,
        [FromQuery] string? sorting,
        [FromQuery] string? search,
        [FromQuery] string[]? filter,
        [FromServices] IFilterParser filterParser,
        [FromServices] IEntityMaterializer materializer,
        [FromServices] IAuditWriter auditWriter,
        [FromKeyedServices(RepositoryKeys.TownLimited)] IRepository repository,
        [FromKeyedServices(RepositoryKeys.TownLimited)] IUnitOfWork unitOfWork,
        [FromQuery] int? page = 1,
        [FromQuery] bool includeTotalCount = false) =>
        {
            var pipeline = PrepareQueryPipeline<TI>(filter, search, viewName, filterParser, materializer);
            if (pipeline.ErrorResult != null)
            {
                return pipeline.ErrorResult;
            }

            var sortingResult = ParseSorting<TI>(sorting);
            if (sortingResult.Error != null)
                return sortingResult.Error;

            long? totalCount = includeTotalCount ? await repository.Count(pipeline.Predicate, pipeline.View) : null;

            try
            {
                var queryResult = await EntityListQueryHelper.GetList(
                    repository,
                    page,
                    perPage,
                    options.DefaultPerPage,
                    options.MaxPerPage,
                    sortingResult.OrderBy,
                    predicate: pipeline.Predicate,
                    view: pipeline.View);
                var data = queryResult.Data;

                await AddReadAuditEvents(data, repository, auditWriter);
                await unitOfWork.Commit();

                var pagedResult = new PagedResult<TI>(data, totalCount);
                return Results.Json(pagedResult, selectSettings);
            }
            catch (ArgumentOutOfRangeException ex) when (ex.ParamName == nameof(page))
            {
                return Results.BadRequest(ex.Message);
            }
        });
    }

    private static void ConfigureCreateEndpoint<T>(
        RouteGroupBuilder entityGroup,
        JsonSerializerSettings updateSettings,
        JsonSerializerOptions selectSettings)
    where T: IEntity
    {
        entityGroup.MapPost("",
        async (HttpRequest request,
            [FromKeyedServices(RepositoryKeys.TownLimited)] IRepository repository,
            [FromKeyedServices(RepositoryKeys.TownLimited)] IUnitOfWork unitOfWork) =>
        {
            var entity = await ReadEntity<T>(request, updateSettings);
            entity.Id = Guid.Empty;
            repository.Create(entity);
            await unitOfWork.Commit();
            var entityPath = $"{request.PathBase}{request.Path}".TrimEnd('/');
            request.HttpContext.Response.Headers.Location = $"{entityPath}/{entity.Id:D}";
            return Results.Json(
                entity,
                selectSettings,
                statusCode: StatusCodes.Status201Created);
        })
        .Accepts(typeof(T), "application/json");
    }

    private static void ConfigureUpdateEndpoint<T>(
        RouteGroupBuilder entityGroup,
        JsonSerializerSettings updateSettings,
        JsonSerializerOptions selectSettings)
    where T: class, IEntity
    {
        entityGroup.MapPut("{id:guid}",
        async (Guid id,
            HttpRequest request,
            [FromKeyedServices(RepositoryKeys.TownLimited)] IRepository repository,
            [FromKeyedServices(RepositoryKeys.TownLimited)] IUnitOfWork unitOfWork) =>
        {
            if (id == Guid.Empty)
            {
                return Results.BadRequest("Идентификатор сущности не может быть пустым");
            }

            if (!await repository.Any<T>(entity => entity.Id == id))
            {
                return Results.NotFound();
            }

            var entity = await ReadEntity<T>(request, updateSettings);
            entity.Id = id;
            repository.Update(entity);
            await unitOfWork.Commit();
            return Results.Json(entity, selectSettings);
        })
        .Accepts(typeof(T), "application/json");
    }

    private static void ConfigureDeleteEndpoint<TI>(RouteGroupBuilder entityGroup)
    where TI: class, IEntity
    {
        entityGroup.MapDelete("{id:guid}",
        async (Guid id,
            [FromKeyedServices(RepositoryKeys.TownLimited)] IRepository repository,
            [FromKeyedServices(RepositoryKeys.TownLimited)] IUnitOfWork unitOfWork) =>
        {
            if (await repository.GetById<TI>(id) == null)
                return Results.NotFound();

            repository.Delete<TI>(id);
            await unitOfWork.Commit();
            return Results.NoContent();
        });
    }

    private static void ConfigureDeleteListEndpoint<TI>(RouteGroupBuilder entityGroup)
    where TI: class, IEntity
    {
        entityGroup.MapPost("deleteByIds",
        async ([FromBody] List<Guid> ids,
            [FromKeyedServices(RepositoryKeys.TownLimited)] IRepository repository,
            [FromKeyedServices(RepositoryKeys.TownLimited)] IUnitOfWork unitOfWork) =>
        {
            ArgumentNullException.ThrowIfNull(ids);
            if (ids.Count == 0)
                return Results.NoContent();

            var distinctIds = ids.Distinct().ToList();
            var available = await repository.GetByIds<TI>(distinctIds);
            if (available.Count != distinctIds.Count)
                return Results.NotFound();

            repository.Delete<TI>(distinctIds);
            await unitOfWork.Commit();
            return Results.NoContent();
        });
    }

    private static async Task<T> ReadEntity<T>(
        HttpRequest request,
        JsonSerializerSettings updateSettings)
    where T: IEntity
    {
        using var stream = new StreamReader(request.Body);
        var entityJson = await stream.ReadToEndAsync();
        return JsonConvert.DeserializeObject<T>(entityJson, updateSettings)
               ?? throw new InvalidCastException("Не удалось получить сущность");
    }

    private static async Task AddReadAuditEvents<TI>(
        List<TI> data,
        IRepository repository,
        IAuditWriter auditWriter)
    where TI: class, IEntity
    {
        foreach (var entity in data)
        {
            await auditWriter.AddEvent(new AuditRequest
            {
                Entity = entity,
                EventType = AuditEventType.Read
            });
        }
    }

    private (Expression<Func<TI, bool>>? Predicate, IResult? Error) BuildPredicate<TI>(
        string? search,
        EntityQueryFilter? filters,
        IEntityView? view)
    where TI: class, IEntity
    {
        Expression<Func<TI, bool>>? predicate = null;

        if (!string.IsNullOrWhiteSpace(search)
            && view?.SearchProperties.Count > 0)
        {
            var searchValue = search.Trim();
            predicate = searchExpressionBuilder.Build<TI>(
                view.SearchProperties,
                searchValue);
        }

        if (filters == null)
        {
            return (predicate, null);
        }

        try
        {
            var filterPredicate = filterExpressionBuilder.Build<TI>(filters);
            predicate = predicate == null
                ? filterPredicate
                : predicate.AndAlso(filterPredicate);
        }
        catch (ArgumentException ex)
        {
            return (null, Results.BadRequest(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return (null, Results.BadRequest(ex.Message));
        }
        catch (NotSupportedException ex)
        {
            return (null, Results.BadRequest(ex.Message));
        }

        return (predicate, null);
    }

    private (Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy, IResult? Error) ParseSorting<T>(
        string? sorting)
        where T : class, IEntity
    {
        try
        {
            return (sortingExpressionBuilder.Build<T>(sorting), null);
        }
        catch (ArgumentException ex)
        {
            return (null, Results.BadRequest(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return (null, Results.BadRequest(ex.Message));
        }
    }

    private static (EntityQueryFilter? Filters, IResult? Error) ParseFilters(
        IFilterParser filterParser,
        string[]? filter)
    {
        try
        {
            var filters = filter is { Length: > 0 }
                ? filterParser.Parse(filter)
                : null;

            return (filters, null);
        }
        catch (ArgumentException ex)
        {
            return (null, Results.BadRequest(ex.Message));
        }
    }

    private static bool IsMethodAllowed(
        InternalEntityAttribute? attribute, EntityVisibility visibility)
    => attribute == null
       || (attribute.Visibility & visibility) != 0;
}
