namespace Tmobis.Business.Process.Documents;

using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Process.Documents.Base;

public interface IBaseAct : IBaseActBase, IWOrganization
{
  string? OrganizationName { get; set; }

  string? OrganizationPhone { get; set; }

  string? OrganizationAddress { get; set; }
}