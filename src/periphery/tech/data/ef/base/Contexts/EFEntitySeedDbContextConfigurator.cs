namespace Tmobis.Data.EF.Contexts;

using Microsoft.EntityFrameworkCore;
using Tmobis.Core.Entities;

/// <summary>
/// Применение дефолтных данных сущностей в EF-модели.
/// </summary>
public class EFEntitySeedDbContextConfigurator(
    IEnumerable<IEntitySeeder> seeders) : IDbContextConfigurator
{
    /// <inheritdoc/>
    public void Build(DbContextOptionsBuilder options)
    {
    }

    /// <inheritdoc/>
    public void Model(ModelBuilder modelBuilder)
    {
        var builder = new EFEntitySeedBuilder(modelBuilder);
        foreach (var seeder in seeders)
        {
            seeder.Seed(builder);
        }
    }
}

file sealed class EFEntitySeedBuilder(ModelBuilder modelBuilder) : IEntitySeedBuilder
{
    /// <inheritdoc/>
    public void Add<TEntity>(params TEntity[] entries) where TEntity : class
    {
        if (entries.Length == 0)
        {
            return;
        }

        modelBuilder.Entity<TEntity>().HasData(entries);
    }
}
