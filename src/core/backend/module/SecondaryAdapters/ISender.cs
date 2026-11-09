namespace Tmobis.Core.SecondaryAdapters;

using System.Collections.Generic;

public interface ISender<T>
{
  public void Send(T message);

  public void SendList(List<T> messages);
}
