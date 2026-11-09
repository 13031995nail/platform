namespace Tmobis.Business.Common.Contacts;

using ICSSoft.STORMNET;

public interface IWOrganization
{
  [PropertyStorage(["Organization"])]
  public IOrganizationBase? Organization { get; set; }
}
