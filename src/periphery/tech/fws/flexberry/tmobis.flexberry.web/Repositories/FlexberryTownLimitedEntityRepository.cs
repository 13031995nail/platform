namespace Tmobis.Frameworks.Flexberry.Web.Repositories;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;
using Tmobis.Frameworks.Flexberry.Repositories;

/// <summary>
/// Entity repository для филиально ограниченного Flexberry DataService.
/// Использует тот же keyed IDataService и DataObjectCache, что и фасад.
/// </summary>
public sealed class FlexberryTownLimitedEntityRepository<TI, T>(
    [FromKeyedServices("Backend")] IDataService dataService,
    [FromKeyedServices(RepositoryKeys.TownLimited)] FlexberryRepositoryContext context)
    : FlexberryEntityRepository<TI, T>(dataService, context)
    where TI : class, IEntity
    where T : DataObject, IStoredEntity, TI, new()
{
}
