namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using Tmobis.Business.Common.Address;
using Tmobis.Frameworks.Flexberry.Entities;
using Tmobis.Core.Entities;

[AutoAltered]
[NotStored(false)]
[ClassStorage("RedinessActObject")]
[AccessType(AccessType.@this)]
[View("AuditView", ["*"])]
[View("RedinessActObjectD", [
  "Act", "Act.Awp", "Act.ActStatus", "Act.ChangeStatusDate", "Act.RegDate",
  "WorkObject", "ObjectReadiness"
])]
[BusinessServer(typeof(RedinessActObjectBS), DataServiceObjectEvents.OnAllEvents)]
public class RedinessActObject : FlexberryEntity
{
  public virtual int? Number { get; set; }
  public virtual LevelReadiness ObjectReadiness { get; set; }

  [NotNull]
  [PropertyStorage(["WorkObject"])]
  public virtual WorkObject? WorkObject { get; set; }

  [NotNull]
  [Agregator]
  [PropertyStorage(["Act"])]
  public virtual RedinessAct? Act { get; set; }

  /// <summary>
  /// Class views container.
  /// </summary>
  public class Views
  {
    public static readonly EntityView RedinessActObjectDEntityView = new(nameof(RedinessActObjectD));

    /// <summary>
    /// "AuditView" view.
    /// </summary>
    public static View AuditView
    => Information.GetView("AuditView",
      typeof(RedinessActObject));

    /// <summary>
    /// "RedinessObjectD" view.
    /// </summary>
    public static View RedinessActObjectD
    => Information.GetView("RedinessActObjectD",
      typeof(RedinessActObject));
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
