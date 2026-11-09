namespace Tmobis.Data.EF.MSSQL;

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tmobis.Core.DI;

public class MSSQLDbContextConfigurator(
    IConfiguration configuration, IOptions<EntityFrameworkOptions> efOptions)
: IDbContextConfigurator
{
    private readonly EntityFrameworkOptions efOptions = efOptions.Value;

    public virtual void Build(DbContextOptionsBuilder options)
    {
        options
        .EnableSensitiveDataLogging()
        .LogTo(message => Console.WriteLine(message), LogLevel.Information);
        var migrationsAssembly = efOptions.MigrationsAssembly
            ?? Assembly.GetExecutingAssembly().GetName().Name;
        var connectionString = configuration.GetDefaultConnectionString();
        options.UseSqlServer(connectionString,
            o => o.MigrationsAssembly(migrationsAssembly))
        .UseLowerCaseNamingConvention();
    }

    public virtual void Model(ModelBuilder modelBuilder)
    {
    }
}
