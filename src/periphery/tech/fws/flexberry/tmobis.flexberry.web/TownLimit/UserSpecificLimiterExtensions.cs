namespace Tmobis.Frameworks.Flexberry.Web.TownLimit;
using System;
using ICSSoft.STORMNET.Business;
using Tmobis.Frameworks.Flexberry.DataServices;
using Tmobis.Frameworks.Flexberry.TownLimit;

public static class UserSpecificLimiterExtensions
{
  public static IDataServiceFactory WithTownLimit(
    this IDataServiceFactory factory, IUserSpecificLimiter userSpecificLimiter)
  {
    ArgumentNullException.ThrowIfNull(factory);

    if (factory.DataService != null
      && factory.DataService is SQLDataService sqlDataService)
    {
      sqlDataService.AddUserSpecificLimit(userSpecificLimiter);
    }

    return factory;
  }

  public static void AddUserSpecificLimit(
    this SQLDataService dataService,
    IUserSpecificLimiter userSpecificLimiter)
  {
    ArgumentNullException.ThrowIfNull(dataService);
    dataService.OnGenerateSQLSelect += (sender, e)
    => userSpecificLimiter
        .AddUserSpecificLimit(e.CustomizationStruct)
        .Wait();
  }
}