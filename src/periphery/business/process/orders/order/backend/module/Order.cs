namespace Tmobis.Business.Process.Orders.Order;

using Core.Models;
using ICSSoft.STORMNET;
using Tmobis.Business.Common.Contacts.Customer.Base;
using Tmobis.Business.Common.Contacts.Supplier.Base;
using Tmobis.Business.Process.Orders.Order.Base;
using Tmobis.Core;

[AutoAltered]
[NotStored(false)]
[ClassStorage("Order")]
[AccessType(AccessType.none)]
[View("OrderE", new[] {
        "Number",
        "WorkObject.Name",
        "Price",
        "StartDate",
        "EndDatePlan",
        "EndDateActual"})]
[View("OrderL", new[] {
    "Number",
    "WorkObject.Name",
    "Price",
    "StartDate",
    "EndDatePlan",
    "EndDateActual"})]
[View("AuditView", new[] { 
    "Creator", 
    "CreateTime", 
    "Editor", 
    "EditTime"})]
[AssociatedDetailView("OrderE", "OrderTools", "OrderToolD",
  true, "", "", true, [""])]
public class Order :
  OrderBase,
  IOrder
{
  public Order()
  {
    // OrderTools = new DetailArray</*I*/OrderTool, /*I*/Order>(this);
  }

  [PropertyStorage(["Supplier"])]
  public virtual /*I*/SupplierBase? Supplier { get; set; }

  [PropertyStorage(["Customer"])]
  [NotNull]
  public virtual /*I*/CustomerBase Customer { get; set; }

  // public virtual DetailArray</*I*/OrderTool, /*I*/Order> OrderTools { get; set; }

  // [PropertyStorage("WorkObject")]
  // public virtual /*I*/IWorkObject WorkObject { get; set; }

  /// <summary>
  /// Class views container.
  /// </summary>
  public class Views
  {

    /// <summary>
    /// "OrderE" view.
    /// </summary>
    public static ICSSoft.STORMNET.View OrderE
    {
      get
      {
        return ICSSoft.STORMNET.Information.GetView("OrderE", typeof(Order));
      }
    }
    public static ICSSoft.STORMNET.View OrderL 
    {
      get
      {
        return ICSSoft.STORMNET.Information.GetView("OrderL", typeof(Order));
      }
    }
    public static ICSSoft.STORMNET.View AuditView 
    {
      get
      {
        return ICSSoft.STORMNET.Information.GetView("AuditView", typeof(Order));
      }
    }
  }

  /// <summary>
  /// Audit class settings.
  /// </summary>
  public class AuditSettings
  {

    /// <summary>
    /// Включён ли аудит для класса.
    /// </summary>
    public static bool AuditEnabled = true;

    /// <summary>
    /// Использовать имя представления для аудита по умолчанию.
    /// </summary>
    public static bool UseDefaultView = false;

    /// <summary>
    /// Включён ли аудит операции чтения.
    /// </summary>
    public static bool SelectAudit = false;

    /// <summary>
    /// Имя представления для аудирования операции чтения.
    /// </summary>
    public static string SelectAuditViewName = "AuditView";

    /// <summary>
    /// Включён ли аудит операции создания.
    /// </summary>
    public static bool InsertAudit = true;

    /// <summary>
    /// Имя представления для аудирования операции создания.
    /// </summary>
    public static string InsertAuditViewName = "AuditView";

    /// <summary>
    /// Включён ли аудит операции изменения.
    /// </summary>
    public static bool UpdateAudit = true;

    /// <summary>
    /// Имя представления для аудирования операции изменения.
    /// </summary>
    public static string UpdateAuditViewName = "AuditView";

    /// <summary>
    /// Включён ли аудит операции удаления.
    /// </summary>
    public static bool DeleteAudit = true;

    /// <summary>
    /// Имя представления для аудирования операции удаления.
    /// </summary>
    public static string DeleteAuditViewName = "AuditView";

    /// <summary>
    /// Путь к форме просмотра результатов аудита.
    /// </summary>
    public static string FormUrl = "";

    /// <summary>
    /// Режим записи данных аудита (синхронный или асинхронный).
    /// </summary>
    public static ICSSoft.STORMNET.Business.Audit.Objects.tWriteMode WriteMode = ICSSoft.STORMNET.Business.Audit.Objects.tWriteMode.Synchronous;

    /// <summary>
    /// Максимальная длина сохраняемого значения поля (если 0, то строка обрезаться не будет).
    /// </summary>
    public static int PrunningLength = 0;

    /// <summary>
    /// Показывать ли пользователям в изменениях первичные ключи.
    /// </summary>
    public static bool ShowPrimaryKey = false;

    /// <summary>
    /// Сохранять ли старое значение.
    /// </summary>
    public static bool KeepOldValue = true;

    /// <summary>
    /// Сжимать ли сохраняемые значения.
    /// </summary>
    public static bool Compress = false;

    /// <summary>
    /// Сохранять ли все значения атрибутов, а не только изменяемые.
    /// </summary>
    public static bool KeepAllValues = false;
  }
}
