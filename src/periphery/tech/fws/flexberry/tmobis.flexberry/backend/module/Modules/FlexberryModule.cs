namespace Tmobis.Frameworks.Flexberry.Modules;

using ICSSoft.Services;
using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using ICSSoft.STORMNET.Business.Audit;
using ICSSoft.STORMNET.Business.Audit.Objects;
using ICSSoft.STORMNET.Business.Interfaces;
using ICSSoft.STORMNET.Security;
using ICSSoft.STORMNET.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewPlatform.Flexberry.AuditBigData;
using NewPlatform.Flexberry.AuditBigData.Serialization;
using NewPlatform.Flexberry.Caching;
using NewPlatform.Flexberry.ORM.CurrentUserService;
using NewPlatform.Flexberry.ORM.ODataService.UserSettingsService;
using NewPlatform.Flexberry.Security;
using NewPlatform.Flexberry.Services;
using System;
using System.Reflection;
using Tmobis.Core.DI;
using Tmobis.Core.Entities;
using Tmobis.Core.Modules;
using Tmobis.Core.Repositories;
using Tmobis.Frameworks.Flexberry.Audit;
using Tmobis.Frameworks.Flexberry.Converters;
using Tmobis.Frameworks.Flexberry.DataServices;
using Tmobis.Frameworks.Flexberry.Entities;
using Tmobis.Frameworks.Flexberry.Passwords;
using Tmobis.Frameworks.Flexberry.Repositories;
using LockService = NewPlatform.Flexberry.Services.LockService;

/// <summary>
/// Модуль интеграции с Flexberry ORM.
/// Содержит Flexberry-реализации сущностей, репозиториев, DataService, security, audit и caches.
/// </summary>
public class FlexberryModule : ModuleBase, IEntityModule, IEntityModuleExternal
{
    /// <inheritdoc/>
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddTransient<ILockService, LockService>();
        services.AddTransient<ICSSoft.STORMNET.Business.LockService>();
        services.AddOrReplaceSingleton<IEntityRegister, FlexberryEntityRegister>();
        services.AddOrReplaceSingleton<
          IPasswordHasher, FlexberryPBKDF2PasswordHasher>();
        services.AddOrReplaceTransient(
          typeof(IEntityRepository<,>), typeof(FlexberryEntityRepository<,>));
        services.AddScoped<FlexberryRepositoryContext>();
        services.AddScoped<FlexberryRepository>();
        services.AddScoped<IReadRepository>(
          provider => provider.GetRequiredService<FlexberryRepository>());
        services.AddScoped<IWriteRepository>(
          provider => provider.GetRequiredService<FlexberryRepository>());
        services.AddScoped<IRepository>(
          provider => provider.GetRequiredService<FlexberryRepository>());
        services.AddScoped<IUnitOfWork>(
          provider => provider.GetRequiredService<FlexberryRepository>());
        services.AddTransient<
          IFlexberryPredicateConverter, FlexberryPredicateConverter>();
        services.AddHttpContextAccessor();
        services.AddSingleton<ICurrentUser, EmptyCurrentUser>();
        RegisterCaches(services, configuration);
        RegisterDataServices(services, configuration);
        RegisterSecurityManagers(services, configuration);
        RegisterAgentManager(services, configuration);
        RegisterAuditManager(services, configuration);
        RegisterBusinessServers(services, configuration, modules);
    }

    /// <inheritdoc/>
    public virtual List<Assembly> GetAdditionalModelAssemblies()
    {
        return
        [
          typeof(IIS.Caseberry.Logging.Objects.ApplicationLog).Assembly,
      typeof(UserSetting).Assembly,
      typeof(Lock).Assembly,
      typeof(AuditEntity).Assembly,
      typeof(Agent).Assembly,
      typeof(AuditRecord).Assembly,
    ];
    }

    protected virtual void RegisterCaches(
      IServiceCollection services,
      IConfiguration configuration)
    {
        services.AddSingleton<ICacheService, MemoryCacheService>();
        services.AddKeyedSingleton<
          ICacheService, MemoryCacheService>("SecurityCache");
        services.AddKeyedSingleton<
          ICacheService, MemoryCacheService>("AgentCache");
    }

    protected virtual void RegisterSecurityManagers(
      IServiceCollection services,
      IConfiguration configuration)
    {
        services.AddSingleton<SecurityBS>();
        services.AddSingleton<FlexberryUserSettingBS>();
        services.AddSingleton<ISecurityManager, EmptySecurityManager>();
        services.AddKeyedSingleton<
          ISecurityManager, SecurityManager>(
          "DataServiceSecurity",
          (reg, key) =>
          {
              var dataService = reg
            .GetRequiredService<IDataService>();
              var cacheManager = reg
            .GetRequiredKeyedService<ICacheService>("SecurityCache");
              var currentUser = reg
            .GetRequiredService<ICurrentUser>();
              return new SecurityManager(dataService, cacheManager, currentUser);
          });
    }

    protected virtual void RegisterAgentManager(
      IServiceCollection services,
      IConfiguration configuration)
    {
        services.AddSingleton<IAgentManager, AgentManager>(
        (reg) =>
        {
            var dataService = reg
          .GetRequiredService<IDataService>();
            var cacheManager = reg
          .GetRequiredKeyedService<ICacheService>("AgentCache");
            var currentUser = reg
          .GetRequiredService<ICurrentUser>();
            var passwordHasher = reg
          .GetRequiredService<IPasswordHasher>();
            return new AgentManager(dataService, cacheManager, currentUser, passwordHasher);
        });
    }

    protected virtual void RegisterAuditManager(
      IServiceCollection services,
      IConfiguration configuration)
    {
        services.AddSingleton<ILegacyAuditSerializer,
          JsonLegacyAuditSerializer>();
        services.AddSingleton<ILegacyAuditConverter,
          LegacyAuditConverter<JsonFieldAuditData>>();
        services.AddSingleton<IAudit, CustomAuditManager>();
        services.AddSingleton<IAuditService, EmptyAuditService>();
        services.AddKeyedSingleton<IAuditService, AuditService>(
        "DataServiceAudit",
        (provider, key) =>
        {
            var currentUser = provider
            .GetRequiredService<ICurrentUser>();
            var auditManager = provider.GetRequiredService<IAudit>();
            var auditAppSetting = new AuditAppSetting
            {
                AppName = "Tmobis",
                AuditEnabled = true,
            };
            var auditService = new AuditService(currentUser)
            {
                Audit = auditManager,
                AppSetting = auditAppSetting,
            };
            return auditService;
        });
    }

    protected virtual void RegisterDataServices(
      IServiceCollection services,
      IConfiguration configuration)
    {
        services.AddTransient<IDataServiceFactory, DataServiceFactory>();

        services.AddSingleton((reg) =>
        {
            var factory = reg.GetRequiredService<IDataServiceFactory>();
            return factory
            .Create()
            .DataService
            ?? throw new InvalidOperationException(
              "Не удалось построить DataService");
        });

        services.AddKeyedSingleton("Audit", (reg, key) =>
        {
            var factory = reg.GetRequiredService<IDataServiceFactory>();
            return factory
            .Create(useAudit: false)
            .DataService
            ?? throw new InvalidOperationException(
              "Не удалось построить DataService");
        });
    }

    protected virtual void RegisterBusinessServers(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        ArgumentNullException.ThrowIfNull(modules);
        foreach (var module in modules)
        {
            var assembly = module.GetType().Assembly;
            assembly
              .GetTypes()
              .Where(type => !type.IsAbstract
                && type.IsAssignableTo(typeof(BusinessServer)))
              .ToList()
              .ForEach(bs => services.AddTransient(bs));
        }

        services.AddSingleton<IBusinessServerProvider>(serviceProvider =>
        {
            return new BusinessServerProvider(serviceProvider);
        });
    }

    // Flexberry.ORM 8.0.0
    public override void Configure(
      IServiceProvider services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Configure(services, configuration, modules);
        SetBackwardCompatibility(services, configuration);
    }

    protected virtual void SetBackwardCompatibility(
      IServiceProvider services,
      IConfiguration configuration)
    {
        var dataService = services.GetService<IDataService>();
        DataServiceProvider.DataService = dataService;
        ExternalLangDef.LanguageDef = new ExternalLangDef(dataService);
        DetailVariableDef.ViewGenerator = null;
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        LogService.LoggerFactory = loggerFactory;
    }
}
