namespace Tmobis.Data.EF.Postgres;

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Tmobis.Core.DI;
using Tmobis.Core.Models;

public class PostgresDbContextConfigurator(
    IConfiguration configuration, IOptions<EntityFrameworkOptions> efOptions)
: IDbContextConfigurator
{
    private readonly EntityFrameworkOptions efOptions = efOptions.Value;

    public virtual void Build(DbContextOptionsBuilder options)
    {
        var migrationsAssembly = efOptions.MigrationsAssembly
            ?? Assembly.GetExecutingAssembly().GetName().Name;
        var connectionString = configuration.GetDefaultConnectionString();
        options.UseNpgsql(connectionString,
            o => o.MigrationsAssembly(migrationsAssembly))
        .UseLowerCaseNamingConvention();
    }

    public void Model(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                var clrProperty = property.PropertyInfo
                    ?? entity.ClrType.GetProperty(property.Name);
                if (clrProperty?.GetCustomAttribute<SerializedJsonAttribute>() != null)
                {
                    property.SetColumnType("jsonb");
                }
            }
        }
    }
}