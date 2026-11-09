namespace Tmobis.Frameworks.Flexberry.Entities;
using ICSSoft.STORMNET;

public class DetailArray<TDetail, TMaster>
: DetailArray, IEnumerable<TDetail>
where TDetail : FlexberryEntity
where TMaster : FlexberryEntity
{
  public DetailArray(TMaster master)
    : base(
      typeof(TDetail),
      typeof(DataObject).GetProperty("__PrimaryKey"),
      master)
  {
  }

  public virtual TDetail this[int index]
  => (TDetail)ItemByIndex(index);

  public virtual void Add(TDetail dataObject)
  => AddObject(dataObject);

  public IEnumerator<TDetail> GetEnumerator()
  => new DetailEnumumerator<TDetail, TMaster>(this);
}

public class DetailEnumumerator<TDetail, TMaster>
: IEnumerator<TDetail>
where TDetail : FlexberryEntity
where TMaster : FlexberryEntity
{
  private readonly DetailArray<TDetail, TMaster> arr;

  private int curIndex;

  public TDetail Current => arr.ItemByIndex(curIndex) as TDetail;

  object System.Collections.IEnumerator.Current => Current;

  public DetailEnumumerator(DetailArray<TDetail, TMaster> array)
  {
    arr = array;
    curIndex = -1;
  }

  public bool MoveNext()
  {
    if (curIndex++ >= arr.Count - 1)
    {
      return false;
    }

    return true;
  }

  public void Reset()
  {
    curIndex = -1;
  }

  public void Dispose()
  {
  }
}