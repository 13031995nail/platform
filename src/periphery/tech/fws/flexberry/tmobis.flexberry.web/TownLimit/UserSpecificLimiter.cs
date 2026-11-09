namespace Tmobis.Frameworks.Flexberry.Web.TownLimit;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ICSSoft.STORMNET.Business;
using ICSSoft.STORMNET.FunctionalLanguage;
using ICSSoft.STORMNET.FunctionalLanguage.SQLWhere;
using Tmobis.Business.Core.TownLimit;
using Tmobis.Business.Core.Models.Towns;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Business.Core.Providers.Users;
using Tmobis.Frameworks.Flexberry.TownLimit;

public class UserSpecificLimiter(
  ICurrentUserProvider currentUserService,
  ITownLimitResolver townLimitResolver)
: IUserSpecificLimiter
{
  /// <summary>
  /// Кеш функций построения ограничений объектов по территории.
  /// </summary>
  // TODO: Заменить на CacheService
  private static readonly ConcurrentDictionary<
    Type, Func<ITown, string, Function>> TownLimitCache = new ();

  private readonly ICurrentUserProvider currentUserService = currentUserService;
  private readonly ITownLimitResolver townLimitResolver = townLimitResolver;

  /// <summary>
  /// Определение необходимости накладывать пользовательское
  /// ограничение при вычитке объектов заданного типа.
  /// </summary>
  /// <param name="type">Тип вычитываемых объектов.</param>
  /// <returns>Нужно ли накладывать дополнительное
  /// пользовательское ограничение.</returns>
  public bool IsNeedUserSpecificLimiting(Type type)
  {
    return townLimitResolver.GetTownLimit(type) != null;
  }

  /// <summary>
  /// Наложить дополнительное ограничение, специфичное для пользователя.
  /// </summary>
  /// <param name="lcs">lcs.</param>
  public async Task AddUserSpecificLimit(LoadingCustomizationStruct lcs)
  {
    if (lcs.LimitFunction == null || NeedFilter(lcs.LimitFunction))
    {
      IUser currentUser = await currentUserService.GetCurrentUser();
      if (lcs.LoadingTypes.Length != 0 && currentUser != null /*&& Town != null*/)
      {
        Type loadingType = lcs.LoadingTypes[0];
        Function townLimitFunction = null;
        var townLimit = townLimitResolver.GetTownLimit(loadingType);

        if (townLimit != null)
        {
          if (!lcs.View.CheckPropname(townLimit.TownPropertyPath) && townLimit.TownPropertyPath != "STORMMainObjectKey")
          {
            lcs.View.AddProperty(townLimit.TownPropertyPath);
          }

          Func<ITown, string, Function> townLimitFunc = null;
          if (!TownLimitCache.TryGetValue(loadingType, out townLimitFunc))
          {
            var method = typeof(UserSpecificLimiter).GetMethod("GetTownLimit", BindingFlags.Static | BindingFlags.Public);
            townLimitFunc = (Func<ITown, string, Function>)Delegate.CreateDelegate(typeof(Func<ITown, string, Function>), method);
            TownLimitCache.TryAdd(loadingType, townLimitFunc);
          }

          townLimitFunction = townLimitFunc(currentUser.Town, townLimit.TownPropertyPath);
        }

        lcs.LimitFunction = FunctionBuilder.BuildAnd(lcs.LimitFunction, townLimitFunction);
      }
    }
  }

  protected bool NeedFilter(Function f)
  {
    Function DoNotFilterFunction = FunctionBuilder.BuildNotEquals("STORMMainObjectKey", Guid.Empty);

    bool res = true;
    if (f.Parameters.Contains(DoNotFilterFunction))
    {
      res = false;
    }
    else
    {
      foreach (Function innerF in f.Parameters.OfType<Function>())
      {
        res = res && NeedFilter(innerF);
        if (!res) break;
      }
    }
    return res;
  }

  public static Function GetTownLimit(ITown town, string field)
  {
    return FunctionBuilder.BuildEquals(
      new VariableDef(SQLWhereLanguageDef.LanguageDef.GuidType, field), town.Id);
  }
}
