namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using Tmobis.Frameworks.Flexberry.Entities;

/// <summary>
/// Акт оценки обеспечения готовности к отопительному периоду.
/// </summary>
[AutoAltered]
[NotStored(false)]
[ClassStorage("RedinessAct")]
[Caption("Акт оценки обеспечения готовности к отопительному периоду")]
[AccessType(AccessType.@this)]
[View("AuditView", ["*"])]
[View("RedinessActE", [
  "RegDate", "ActStatus", "ActNumber", "ChangeStatusDate", "OrganizationName",
  "OrganizationPhone", "OrganizationAddress", "ActNumberIndex",
  "Assignee", "ActType", "Organization", "Awp",
  "AwpAssessingProgram", "PartnerReadiness"
])]
[AssociatedDetailView("RedinessActE", "Objects", "RedinessActObjectD", true, "", "", true, [""])]
[View("RedinessActL", [
  "RegDate", "ActStatus", "ActNumber", "ChangeStatusDate", "OrganizationName",
  "OrganizationPhone", "OrganizationAddress", "ActNumberIndex",
  "Assignee", "ActType", "Organization", "Awp",
  "AwpAssessingProgram", "PartnerReadiness"
])]
[BusinessServer(typeof(RedinessActBS), DataServiceObjectEvents.OnAllEvents)]
public class RedinessAct : RedinessActBase
{
  public RedinessAct()
  {
    Objects = new(this);
    ActSigns = new(this);
  }

  [PropertyStorage(["AwpAssessingProgram"])]
  public virtual AwpAssessingProgram AwpAssessingProgram { get; set; }

  public virtual PartnerReadinessLevel PartnerReadiness { get; set; }

  public virtual decimal AvgObjectReadinessIndex { get; set; } = 0m;

  public virtual DetailArray<RedinessActObject, RedinessAct> Objects { get; set; }
  public virtual DetailArray<ReadinessActSign, RedinessAct> ActSigns { get; set; }

  /// <summary>
  /// Class views container.
  /// </summary>
  public new class Views
  {
      public static readonly Tmobis.Core.Entities.EntityView RedinessActLEntityView = new(nameof(RedinessActL));
    /// <summary>
    /// "AuditView" view.
    /// </summary>
    public static View AuditView
    => Information.GetView("AuditView", typeof(RedinessAct));

    /// <summary>
    /// "RedinessActL" view.
    /// </summary>
    public static View RedinessActE
    => Information.GetView("RedinessActE", typeof(RedinessAct));

    /// <summary>
    /// "RedinessActL" view.
    /// </summary>
    public static View RedinessActL
    => Information.GetView("RedinessActL", typeof(RedinessAct));
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
