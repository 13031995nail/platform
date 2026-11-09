namespace Tmobis.Data.EF.Contexts;

using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Tmobis.Core.Entities;
using Tmobis.Core.Models;

public class DatabaseContext
: DbContext, IDbContext
{
    private readonly IEntityMaterializer materializer;
    private readonly IEnumerable<IDbContextConfigurator> dbContextConfigurators;

    public DatabaseContext(
        IEntityMaterializer materializer,
        IEnumerable<IDbContextConfigurator> dbContextConfigurators)
    {
        this.materializer = materializer;
        this.dbContextConfigurators = dbContextConfigurators;
    }

    public DatabaseContext(
        IEntityMaterializer materializer,
        IEnumerable<IDbContextConfigurator> dbContextConfigurators,
        DbContextOptions<DatabaseContext> options)
        : base(options)
    {
        this.materializer = materializer;
        this.dbContextConfigurators = dbContextConfigurators;
    }

    /// <inheritdoc/>
    public virtual DbSet<TModel> GetTable<TModel>()
    where TModel: class, IEntity
    => Set<TModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var mapping in materializer.StoredMaterializations)
        {
            var iface = mapping.Key;
            var materialized = mapping.Value;
            var entityBuilder = modelBuilder
            .Entity(materialized)
            .ToTable(materialized.Name.ToLower());

            var entityProps = GetMappedProperties(materialized)
            .Where(prop => !prop.PropertyType.IsClass
                && prop.PropertyType.IsAssignableTo(typeof(IEntity)))
            .ToList();

            foreach (var prop in entityProps)
            {
                var propType = prop.PropertyType;
                var materializedProp = materializer
                    .ResolveStoredEntityType(propType);
                
                // Поиск агрегации (списка) для данной сущности
                // с другой стороны ассоциации. Если найдется, то
                // будет установлена связь в обе стороны.
                var aggregationPropType = GetMappedProperties(propType)
                .FirstOrDefault(prop => prop.PropertyType.IsGenericType
                    && prop.PropertyType.IsAssignableTo(typeof(IEnumerable))
                    && !prop.PropertyType.GenericTypeArguments[0].IsClass
                    && prop.PropertyType.GenericTypeArguments[0]
                    .IsAssignableTo(iface))?
                .Name;

                // TODO: может, есть более простой способ задания ссылок
                // TODO: через конвенции?
                entityBuilder
                .HasOne(materializedProp, prop.Name)
                .WithMany(aggregationPropType);
            }
        }
        
        foreach (var configurator in dbContextConfigurators)
        {
            configurator.Model(modelBuilder);
        }
    }

    protected virtual List<PropertyInfo> GetMappedProperties(Type type)
    => [.. type.GetProperties()
        .Where(prop => prop
            .GetCustomAttribute<NotMappedAttribute>() == null)];
}