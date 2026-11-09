namespace Tmobis.Frameworks.Flexberry.TownLimit;
using System;
using ICSSoft.STORMNET.Business;

public interface IUserSpecificLimiter
{
  public bool IsNeedUserSpecificLimiting(Type type);

  public Task AddUserSpecificLimit(LoadingCustomizationStruct lcs);

  //public Function GetTownLimit(Town town, string field);
}
