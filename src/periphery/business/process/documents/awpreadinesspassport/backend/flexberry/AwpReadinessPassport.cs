namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Frameworks.Flexberry.Entities;

/// <summary>
/// Паспорт обеспечения готовности к отопительному периоду.
/// </summary>
[AutoAltered]
[NotStored(false)]
[ClassStorage("AwpReadinessPassport")]
[Caption("Паспорт обеспечения готовности к отопительному периоду")]
[AccessType(AccessType.@this)]
[View("AuditView", ["*"])]
[View("AwpReadinessPassportE", [
  "RegDate", "ActStatus", "ActNumber", "ChangeStatusDate", "OrganizationName",
  "OrganizationPhone", "OrganizationAddress", "ActNumberIndex",
  "Assignee", "ActType", "Organization", "Awp",
  "ReadinessAct", "PartnerReadiness"
])]
[AssociatedDetailView("AwpReadinessPassportE", "Objects", "AwpReadinessPassportObjectE", true, "", "", true, [""])]
[View("AwpReadinessPassportL", [
  "RegDate", "ActStatus", "ActNumber", "ChangeStatusDate", "OrganizationName",
  "OrganizationPhone", "OrganizationAddress", "ActNumberIndex",
  "Assignee", "ActType", "Organization", "Awp",
  "ReadinessAct", "PartnerReadiness"
])]
[BusinessServer(typeof(AwpReadinessPassportBS), DataServiceObjectEvents.OnAllEvents)]
public class AwpReadinessPassport : BaseAct, IWAwp
{
  public AwpReadinessPassport()
  {
    Objects = new(this);
    ActSigns = new(this);
  }

  [PropertyStorage(["ReadinessAct"])]
  public virtual RedinessAct? ReadinessAct { get; set; }

  public virtual PartnerReadinessLevel PartnerReadiness { get; set; }

  [NotNull]
  [PropertyStorage(["Awp"])]
  public virtual Awp? Awp { get; set; }

  public virtual DetailArray<AwpReadinessPassportObject,
        AwpReadinessPassport> Objects
  { get; set; }

  public virtual DetailArray<AwpReadinessPassportSign, AwpReadinessPassport> ActSigns { get; set; }

  /// <summary>
  /// Class views container.
  /// </summary>
  public new class Views
  {
    /// <summary>
    /// "AuditView" view.
    /// </summary>
    public static View AuditView
    => Information.GetView("AuditView", typeof(AwpReadinessPassport));


    /// <summary>
    /// "AwpReadinessPassportE" view.
    /// </summary>
    public static View AwpReadinessPassportE
    => Information.GetView("AwpReadinessPassportE", typeof(AwpReadinessPassport));

    /// <summary>
    /// "AwpReadinessPassportL" view.
    /// </summary>
    public static View AwpReadinessPassportL
    => Information.GetView("AwpReadinessPassportL", typeof(AwpReadinessPassport));
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
