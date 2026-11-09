namespace Tmobis.Business.Common.Resources.WorkType;

using ICSSoft.STORMNET;

public interface IWWorkType
{
  [PropertyStorage(["WorkType"])]
  [NotNull]
  public IWorkType WorkType { get; set; }
}
