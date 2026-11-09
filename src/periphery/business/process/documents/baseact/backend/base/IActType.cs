namespace Tmobis.Business.Process.Documents;
using Tmobis.Core.Models;

public interface IActType : IDictionary, IRegisteredEntity
{
  string Acronym { get; set; }

  string ShortName { get; set; }

  string EditFormName { get; set; }
}