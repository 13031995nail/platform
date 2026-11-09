namespace Tmobis.Frameworks.Flexberry.Linq;
using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using ICSSoft.STORMNET.Business.LINQProvider;
using Tmobis.Core.Models;

public static class FlexberryLinqExtensions
{
  public static IQueryable<T> QueryExt<T>(
    this IDataService dataService, View view)
    where T : DataObject, IStoredEntity
    => new LcsQuery<T, FlexberryLcsGeneratorQueryModelVisitor>(
        new LcsQueryProvider<T, FlexberryLcsGeneratorQueryModelVisitor>(
            dataService, view, null));
}