namespace Tmobis.Business.Process.Documents.Base.Flexberry;

using System.Runtime.Serialization;
using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using Tmobis.Business.Core.Flexberry.Models;
using Tmobis.Business.Process.Documents.Base;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Flexberry.Autoincrement.Objects;
using Tmobis.Frameworks.Flexberry.Entities;

/// <summary>
/// Документ.
/// </summary>
[ClassStorage("BaseAct")]
[AutoAltered]
[Caption("Документ")]
[AccessType(AccessType.none)]
[View("BaseActBaseE", [
  "RegDate", "ActStatus", "ActNumber", "ChangeStatusDate", "ActNumberIndex",
  "Assignee", "ActType"
])]
[BusinessServer(typeof(BaseActBaseBS), DataServiceObjectEvents.OnAllEvents)]
public abstract class BaseActBase
: FlexberryEntity, IBaseActBase, IAutoincremented
{
  [NotNull]
  public virtual DateTime RegDate { get; set; }

  [NotNull]
  [PropertyStorage(["ActStatus"])]
  public virtual ActStatus ActStatus { get; set; }

  [NotNull]
  [PropertyStorage(["ActType"])]
  public virtual ActType ActType { get; set; }

  [NotNull]
  [StrLen(50)]
  public virtual string ActNumber { get; set; }

  public virtual DateTime? ChangeStatusDate { get; set; }

  public virtual int? ActNumberIndex { get; set; }

  [NotNull]
  [PropertyStorage(["Assignee"])]
  public virtual Employee? Assignee { get; set; }

  [NotStored]
  [IgnoreDataMember]
  IActStatus IBaseActBase.Status
  {
    get => ActStatus;
    set
    {
      if (value is ActStatus status)
      {
        ActStatus = status;
      }
    }
  }

  [NotStored]
  [IgnoreDataMember]
  IActType IBaseActBase.ActType
  {
    get => ActType;
    set
    {
      if (value is ActType type)
      {
        ActType = type;
      }
    }
  }

  [NotStored]
  [IgnoreDataMember]
  Core.Models.Employees.IEmployee? IBaseActBase.Assignee 
  {
    get => Assignee;
    set
    {
      if (value is Employee assignee)
      {
        Assignee = assignee;
      }
    }
  }

  /// <summary>
  /// Ключ счетчика автоинкремента.
  /// </summary>
  [NotStored]
  public virtual string CounterKey
  => $"{CreateTime?.Year}_{GetType().Name}{
    (Assignee?.Town != null ? $"_{Assignee?.Town?.Id}" : "")}";

  /// <summary>
  /// Человекочитаемое имя счетчика автоинкремента.
  /// </summary>
  [NotStored]
  public virtual string CounterName
  => CounterKey;

  /// <summary>
  /// Режим автоинкремента.
  /// </summary>
  [NotStored]
  public virtual AutoincrementMode AutoincrementMode
  => AutoincrementMode.Automatic;

  /// <summary>
  /// Установить значение в автоинкрементируемое свойство.
  /// </summary>
  /// <param name="value">The value.</param>
  public void SetAutoincrementedFieldValue(int value)
  {
    ActNumberIndex = value;
  }

  /// <summary>
  /// Class views container.
  /// </summary>
  public class Views
  {
    /// <summary>
    /// "BaseActBaseE" view.
    /// </summary>
    public static View BaseActBaseE
      => Information.GetView("BaseActBaseE", typeof(BaseActBase));
  }
}
