namespace Tmobis.Business.Common.Personnel;
using ICSSoft.STORMNET;

/// <summary>
/// Статус акта.
/// </summary>
public enum AdmissionCommitteeStatus
{
  /// <summary>
  /// На формировании
  /// </summary>
  [Caption("На формировании")]
  Editing,

  /// <summary>
  /// Утверждена
  /// </summary>
  [Caption("Утверждена")]
  Approved,
}
