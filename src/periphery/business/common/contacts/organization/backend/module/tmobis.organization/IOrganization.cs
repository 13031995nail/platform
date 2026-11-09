namespace Tmobis.Business.Common.Contacts;

public interface IOrganization : IOrganizationBase
{
  IOrganizationBase? Hierarchy { get; set; }
}
