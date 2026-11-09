namespace Tmobis.Data.EF;

using Audit;
using Contexts;
using Core.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories;
using Tmobis.Core.Modules;
using Tmobis.Core.Repositories;

/// <summary>
/// Базовый модуль Entity Framework.
/// Содержит DbContext, обработку изменений, town-limit policies и EF-реализации репозиториев.
/// </summary>
[ModuleRequirement([typeof(AuditModule)])]
public class EntityFrameworkModule : ModuleBase
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);

        services.Configure<EntityFrameworkOptions>(
            configuration.GetSection(
                EntityFrameworkOptions.EntityFramework));

        services.AddTransient<
            IDbContextConfigurator,
            EmptyDbContextConfigurator>();

        services.AddTransient<
            IDbContextConfigurator,
            EFEntitySeedDbContextConfigurator>();

        services.AddDbContext<IDbContext, DatabaseContext>(
            (provider, options) =>
            {
                var ctxBuilders =
                    provider.GetServices<IDbContextConfigurator>();

                foreach (var ctxBuilder in ctxBuilders)
                {
                    ctxBuilder.Build(options);
                }
            });

        services.AddScoped<
            IEntityRepositoryResolver,
            EntityRepositoryResolver>();

        services.AddScoped<
            IEFEntityViewLoader,
            EFEntityViewLoader>();

        services.AddScoped<
            IEFEntityChangeProcessor,
            EFEntityChangeProcessor>();

        services.AddScoped(
            typeof(IEFEntityAccessPolicy<,>),
            typeof(EFTownLimitPolicy<,>));

        services.AddOrReplaceTransient(
            typeof(IEntityRepository<,>),
            typeof(EFUnrestrictedEntityRepository<,>));

        services.AddKeyedTransient(
            typeof(IEntityRepository<,>),
            RepositoryKeys.TownLimited,
            typeof(EFEntityRepository<,>));

        services.AddScoped<EFRepository>();
        services.AddScoped<IReadRepository>(
            provider => provider.GetRequiredService<EFRepository>());
        services.AddScoped<IWriteRepository>(
            provider => provider.GetRequiredService<EFRepository>());
        services.AddScoped<IRepository>(
            provider => provider.GetRequiredService<EFRepository>());
        services.AddScoped<IUnitOfWork>(
            provider => provider.GetRequiredService<EFRepository>());

        services.AddKeyedScoped<EFTownLimitedRepository>(
            RepositoryKeys.TownLimited);
        services.AddKeyedScoped<IReadRepository>(
            RepositoryKeys.TownLimited,
            (provider, _) => provider.GetRequiredKeyedService<EFTownLimitedRepository>(
                RepositoryKeys.TownLimited));
        services.AddKeyedScoped<IWriteRepository>(
            RepositoryKeys.TownLimited,
            (provider, _) => provider.GetRequiredKeyedService<EFTownLimitedRepository>(
                RepositoryKeys.TownLimited));
        services.AddKeyedScoped<IRepository>(
            RepositoryKeys.TownLimited,
            (provider, _) => provider.GetRequiredKeyedService<EFTownLimitedRepository>(
                RepositoryKeys.TownLimited));
        services.AddKeyedScoped<IUnitOfWork>(
            RepositoryKeys.TownLimited,
            (provider, _) => provider.GetRequiredKeyedService<EFTownLimitedRepository>(
                RepositoryKeys.TownLimited));
    }
}
