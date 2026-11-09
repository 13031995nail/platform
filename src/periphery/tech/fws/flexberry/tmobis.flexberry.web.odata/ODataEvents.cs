namespace Tmobis.Frameworks.Flexberry.Web.OData;

using System;
using System.Linq;
using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using Tmobis.Core.Medium.Producers;
using Tmobis.Frameworks.Flexberry.Web.OData.Messages;

/// <summary>
/// События OData.
/// </summary>
public class ODataEvents
{
  // TODO: DI
  public static IMessageDispatcher MessageDispatcher { get; set; }

  /// <summary>
  /// Метод, вызываемый перед вычитыванием объектов данных.
  /// </summary>
  /// <param name="objs">Объекты данных.</param>
  public static bool BeforeGet(ref LoadingCustomizationStruct lcs)
  {
    var task = MessageDispatcher.Send(new ODataBeforeGetMessage() { Lcs = lcs });
    task.Wait();

    return true;
  }

  /// <summary>
  /// Метод, вызываемый после вычитывания объектов данных.
  /// </summary>
  /// <param name="objs">Объекты данных.</param>
  public static void AfterGet(ref DataObject[] objs)
  {
    MessageDispatcher.Send(new ODataAfterGetMessage() { Objects = objs?.ToList() });
  }

  /// <summary>
  /// Метод, вызываемый перед созданием объекта.
  /// </summary>
  /// <param name="obj">Объект данных.</param>
  public static bool BeforeCreate(DataObject obj)
  {
    MessageDispatcher.Send(new ODataBeforeCreateMessage() { Object = obj });

    return true;
  }

  /// <summary>
  /// Метод, вызываемый после создания объекта.
  /// </summary>
  /// <param name="obj">Объект данных.</param>
  public static void AfterCreate(DataObject obj)
  {
    MessageDispatcher.Send(new ODataAfterCreateMessage() { Object = obj });
  }

  /// <summary>
  /// Метод, вызываемый перед обновлением объекта.
  /// </summary>
  /// <param name="obj">Объект данных.</param>
  public static bool BeforeUpdate(DataObject obj)
  {
    var task = MessageDispatcher.Send(new ODataBeforeUpdateMessage() { Object = obj });
    task.Wait();

    return true;
  }
}
