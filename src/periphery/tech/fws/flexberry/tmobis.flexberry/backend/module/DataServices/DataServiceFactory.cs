namespace Tmobis.Frameworks.Flexberry.DataServices;

using System.Globalization;
using ICSSoft.STORMNET.Business;
using ICSSoft.STORMNET.Business.Audit;
using ICSSoft.STORMNET.Business.Interfaces;
using ICSSoft.STORMNET.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.DI;

public class DataServiceFactory(
  [FromKeyedServices("Template")] IDataService templateDataService,
  IServiceProvider provider,
  ISecurityManager emptySecurity,
  IAuditService emptyAudit,
  IConfiguration configuration)
: IDataServiceFactory
{
  private readonly IServiceProvider provider = provider;
  private readonly IDataService templateDataService = templateDataService;
  private readonly ISecurityManager emptySecurity = emptySecurity;
  private readonly IAuditService emptyAudit = emptyAudit;
  private readonly IConfiguration configuration = configuration;

  public virtual IDataService? DataService { get; protected set; }

  // TODO: Найти способ задавать аудит через WithAudit
  // сейчас мешает отсутствие сеттера у AuditService
  public virtual IDataServiceFactory Create(
    bool setDefaultConnectionString = true, bool useAudit = true)
  {
    var auditService = useAudit
      ? provider.GetKeyedService<IAuditService>("DataServiceAudit")
      : emptyAudit;
    var bsp = provider.GetService<IBusinessServerProvider>();

    DataService = Activator.CreateInstance(
      templateDataService.GetType(),
      [emptySecurity, auditService, bsp]) as IDataService;

    if (DataService != null)
    {
      if (DataService is SQLDataService sqlDS)
      {
        var commandTimeout = int.Parse(
          configuration["SqlCommandTimeout"] ?? "30",
          CultureInfo.InvariantCulture);

        sqlDS.UseCommandTimeout = true;
        sqlDS.CommandTimeout = commandTimeout;
      }

      if (setDefaultConnectionString)
      {
        WithConnectionString(GetDefaultConnectionString());
      }

      var dsType = DataService.GetType();
      var prop = dsType.GetProperty("SecurityManager");
      prop?.SetValue(DataService, emptySecurity);
    }

    return this;
  }

  protected virtual string GetDefaultConnectionString()
  {
    var connectionString = configuration.GetDefaultConnectionString();
    if (string.IsNullOrEmpty(connectionString))
    {
      throw new InvalidOperationException(
        "Строка подключения по умолчанию не найдена");
    }

    return connectionString;
  }

  public virtual IDataServiceFactory WithConnectionString(string connectionString)
  {
    DataService?.CustomizationString = connectionString;
    return this;
  }

  public virtual IDataServiceFactory WithSecurity()
  {
    // TODO: Избавиться от ServiceLocator?
    // Напрямую не получается из-за циклической зависимости
    var securityManager = provider
      .GetKeyedService<ISecurityManager>("DataServiceSecurity");
    if (DataService == null || securityManager == null)
    {
      return this;
    }

    var prop = DataService.GetType().GetProperty("SecurityManager");
    if (prop == null)
    {
      return this;
    }

    prop.SetValue(DataService, securityManager);
    return this;
  }

  // public IDataServiceFactory WithAudit()
  // {
    
  // }
}
