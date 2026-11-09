namespace Tmobis.Business.Common.Resources;

using ICSSoft.STORMNET;
using Tmobis.Frameworks.Flexberry.Entities;
using Tmobis.Core.Entities;
using ICSSoft.STORMNET.Business;

/// <summary>
/// Осенне-зимний период.
/// </summary>
[AutoAltered]
[NotStored(false)]
[Caption("Осенне-зимний период")]
[AccessType(AccessType.@this)]
[View("AuditView", [
    "Creator", "CreateTime", "Editor", "EditTime", "DateBegin", "DateEnd", "IsCurrent", "Actuality"
])]
[View("AwpE", [
    "Name", "DateBegin", "DateEnd", "IsCurrent", "Actuality"
])]
[View("AwpL", [
    "DateBegin", "DateEnd", "IsCurrent", "Actuality"
])]
[BusinessServer(typeof(AwpBS), DataServiceObjectEvents.OnAllEvents)]
[ClassStorage("Awp")]
public class Awp : FlexberryActualEntity
{
  [NotNull]
  public virtual DateTime DateBegin { get; set; }

  [NotNull]
  public virtual DateTime DateEnd { get; set; }

  [NotNull]
  public virtual bool IsCurrent { get; set; } = false;

  [StrLen(150)]
  public virtual string Name { get; set; }

  /// <summary>
  /// Class views container.
  /// </summary>
  public class Views
  {
    public static readonly EntityView AwpEEntityView = new(nameof(AwpE));

    /// <summary>
    /// "AuditView" view.
    /// </summary>
    public static View AuditView
      => Information.GetView("AuditView", typeof(Awp));

    /// <summary>
    /// "AwpE" view.
    /// </summary>
    public static View AwpE
              => Information.GetView("AwpE", typeof(Awp));

    /// <summary>
    /// "AwpL" view.
    /// </summary>
    public static View AwpL
              => Information.GetView("AwpL", typeof(Awp));
  }
  /// <summary>
  /// Audit class settings.
  /// </summary>
  public class AuditSettings
  {
    /// <summary>
    /// Включён ли аудит для класса.
    /// </summary>
    public static readonly bool AuditEnabled = true;

    /// <summary>
    /// Использовать имя представления для аудита по умолчанию.
    /// </summary>
    public static readonly bool UseDefaultView = false;

    /// <summary>
    /// Включён ли аудит операции чтения.
    /// </summary>
    public static readonly bool SelectAudit = false;

    /// <summary>
    /// Имя представления для аудирования операции чтения.
    /// </summary>
    public static readonly string SelectAuditViewName = "AuditView";

    /// <summary>
    /// Включён ли аудит операции создания.
    /// </summary>
    public static readonly bool InsertAudit = true;

    /// <summary>
    /// Имя представления для аудирования операции создания.
    /// </summary>
    public static readonly string InsertAuditViewName = "AuditView";

    /// <summary>
    /// Включён ли аудит операции изменения.
    /// </summary>
    public static readonly bool UpdateAudit = true;

    /// <summary>
    /// Имя представления для аудирования операции изменения.
    /// </summary>
    public static readonly string UpdateAuditViewName = "AuditView";

    /// <summary>
    /// Включён ли аудит операции удаления.
    /// </summary>
    public static readonly bool DeleteAudit = true;

    /// <summary>
    /// Имя представления для аудирования операции удаления.
    /// </summary>
    public static readonly string DeleteAuditViewName = "AuditView";

    /// <summary>
    /// Путь к форме просмотра результатов аудита.
    /// </summary>
    public static readonly string FormUrl = "";

    /// <summary>
    /// Режим записи данных аудита (синхронный или асинхронный).
    /// </summary>
    public static readonly ICSSoft.STORMNET.Business.Audit.Objects.tWriteMode WriteMode
        = ICSSoft.STORMNET.Business.Audit.Objects.tWriteMode.Synchronous;

    /// <summary>
    /// Максимальная длина сохраняемого значения поля (если 0, то строка обрезаться не будет).
    /// </summary>
    public static readonly int PrunningLength = 0;

    /// <summary>
    /// Показывать ли пользователям в изменениях первичные ключи.
    /// </summary>
    public static readonly bool ShowPrimaryKey = false;

    /// <summary>
    /// Сохранять ли старое значение.
    /// </summary>
    public static readonly bool KeepOldValue = true;

    /// <summary>
    /// Сжимать ли сохраняемые значения.
    /// </summary>
    public static readonly bool Compress = false;

    /// <summary>
    /// Сохранять ли все значения атрибутов, а не только изменяемые.
    /// </summary>
    public static readonly bool KeepAllValues = false;
  }
}

