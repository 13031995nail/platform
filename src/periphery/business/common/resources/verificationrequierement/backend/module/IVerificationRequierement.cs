namespace Tmobis.Business.Common.Resources;
using Tmobis.Core.Models;
using Tmobis.Business.Common.Contacts;

public interface IVerificationRequierement
: IDictionary, IRegisteredEntity
{
  public bool IsCalculated { get; set; }

  //NotNull]
  //[PropertyStorage(new string[] { "OrganizationType" })]
  public OrganizationType OrganizationType { get; set; }

  //[StrLen(250)]
  public string ShortReqName { get; set; }

  //[NotNull]
  //[StrLen(10)]
  public string ReqNumber { get; set; }

  //[StrLen(15)]
  public string ShortIndicatorName { get; set; }

  //[NotNull]
  //[StrLen(250)]
  public string IndicatorName { get; set; }

  //[StrLen(250)]
  public string SupportingDocName { get; set; }

  public System.Decimal? Weight { get; set; }
}
