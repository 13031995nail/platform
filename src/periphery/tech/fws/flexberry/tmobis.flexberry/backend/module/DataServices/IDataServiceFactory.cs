namespace Tmobis.Frameworks.Flexberry.DataServices;
using ICSSoft.STORMNET.Business;

public interface IDataServiceFactory
{
  public IDataService? DataService { get; }

  public IDataServiceFactory Create(
    bool setDefaultConnectionString = true, bool useAudit = true);

  public IDataServiceFactory WithConnectionString(string connectionString);

  public IDataServiceFactory WithSecurity();

  // public IDataServiceFactory WithAudit();
}
