namespace Tmobis.Business.Process.Orders.Order.Base;

using ICSSoft.STORMNET;
using Tmobis.Frameworks.Flexberry.Entities;

[ClassStorage("Order")]
public class OrderBase :
  FlexberryEntity,
  IOrder
{
  [PropertyStorage("Number")]
  public string Number { get; set; }

  [PropertyStorage("Price")]
  public decimal Price { get; set; }

  [PropertyStorage("StartDate")]
  public DateTime StartDate { get; set; }

  [PropertyStorage("EndDatePlan")]
  public DateTime EndDatePlan { get; set; }

  [PropertyStorage("EndDateActual")]
  public DateTime? EndDateActual { get; set; }

  private DateTime? fCreateTime;
  private string fCreator;
  private string fEditor;
  private DateTime? fEditTime;

  /// <summary>
  /// Время создания объекта.
  /// </summary>
  public virtual DateTime? CreateTime
  {
      get => fCreateTime;
      set => fCreateTime = value;
  }

  /// <summary>
  /// Создатель объекта.
  /// </summary>
  [StrLen(255)]
  public virtual string Creator
  {
      get => fCreator;
      set => fCreator = value;
  }

  /// <summary>
  /// Последний редактор объекта.
  /// </summary>
  [StrLen(255)]
  public virtual string Editor
  {
      get => fEditor;
      set => fEditor = value;
  }

  /// <summary>
  /// Время последнего редактирования объекта.
  /// </summary>
  public virtual DateTime? EditTime
  {
      get => fEditTime;
      set => fEditTime = value;
  }
}
