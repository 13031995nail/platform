namespace Tmobis.Core.PrimaryAdapters;

using System;

public interface IListener : IDisposable
{
  public void StartListener();

  public void CloseListener();
}
