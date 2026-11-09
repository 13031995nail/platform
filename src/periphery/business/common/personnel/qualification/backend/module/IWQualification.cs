namespace Tmobis.Business.Common.Personnel;

using ICSSoft.STORMNET;

public interface IWQualification
{
  [NotNull]
  [PropertyStorage("Qualification")]
  IQualification Qualification { get; set; }
}
