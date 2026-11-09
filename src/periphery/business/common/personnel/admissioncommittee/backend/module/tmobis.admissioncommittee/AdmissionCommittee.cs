namespace Tmobis.Business.Common.Personnel;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Common.Resources;
using Tmobis.Frameworks.Flexberry.Entities;

/// <summary>
/// Комиссия.
/// </summary>
[AutoAltered]
[NotStored(false)]
[ClassStorage("AdmissionCommittee")]
[AccessType(AccessType.@this)]
[View("AdmissionCommitteeE", [
    "Organization", "Awp", "Status",
    "Leader", "Leader.Employee", "Leader.Employee.LastName", "Leader.Employee.FirstName", "Leader.Employee.Patronymic", "Leader.Employee.Fio",
    "DeputyLeader", "DeputyLeader.Employee", "DeputyLeader.Employee.LastName", "DeputyLeader.Employee.FirstName", "DeputyLeader.Employee.Patronymic", "DeputyLeader.Employee.Fio"
])]
[View("AdmissionCommitteeL", [
    "Organization", "Awp", "Leader"
])]
[View("AuditView", [
    "Organization", "Awp", "Leader", "DeputyLeader"
])]
[AssociatedDetailView("AdmissionCommitteeE", "AdmissionCommitteeEmployee", "AdmissionCommitteeEmployeeD", true, "", "", true, [""])]
[BusinessServer(typeof(AdmissionCommitteeBS), DataServiceObjectEvents.OnAllEvents)]
public class AdmissionCommittee :
  FlexberryEntity
{
  public AdmissionCommittee()
  {
    AdmissionCommitteeEmployee = new(this);
  }

  [PropertyStorage(["Leader"])]
  public virtual AdmissionCommitteeEmployee? Leader { get; set; }

  public virtual DateTime? ApprovedDate { get; set; }

  [PropertyStorage(["DeputyLeader"])]
  public virtual AdmissionCommitteeEmployee? DeputyLeader { get; set; }

  [NotNull]
  public virtual DateTime RegDate { get; set; }

  [NotNull]
  [PropertyStorage(["Organization"])]
  public virtual Organization Organization { get; set; }

  [NotNull]
  [PropertyStorage(["Awp"])]
  public virtual Awp Awp { get; set; }

  public virtual AdmissionCommitteeStatus Status { get; set; } = AdmissionCommitteeStatus.Editing;

  public virtual DetailArray<AdmissionCommitteeEmployee, AdmissionCommittee> AdmissionCommitteeEmployee { get; set; }

  /// <summary>
  /// Class views container.
  /// </summary>
  public class Views
  {
    /// <summary>
    /// "AuditView" view.
    /// </summary>
    public static View AuditView
      => Information.GetView("AuditView", typeof(AdmissionCommittee));

    /// <summary>
    /// "AdmissionCommitteeE" view.
    /// </summary>
    public static View AdmissionCommitteeE
              => Information.GetView("AdmissionCommitteeE", typeof(AdmissionCommittee));

    /// <summary>
    /// "AdmissionCommitteeL" view.
    /// </summary>
    public static View AdmissionCommitteeL
              => Information.GetView("AdmissionCommitteeL", typeof(AdmissionCommittee));
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
