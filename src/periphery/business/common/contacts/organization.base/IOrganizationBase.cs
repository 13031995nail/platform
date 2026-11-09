namespace Tmobis.Business.Common.Contacts;

using Tmobis.Core.Models;

public interface IOrganizationBase : IRegisteredEntity
{
  string Name { get; set; }

  string Address { get; set; }

  string LegalAddress { get; set; }

  string Phone { get; set; }

  string Email { get; set; }

  string Acronym { get; set; }
}
