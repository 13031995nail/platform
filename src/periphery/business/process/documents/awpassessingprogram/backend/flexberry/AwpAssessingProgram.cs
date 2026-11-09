namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using Tmobis.Business.Common.Personnel;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Frameworks.Flexberry.Entities;

/// <summary>
/// Программа проведения оценки обеспечения готовности к ОП.
/// </summary>
[AutoAltered]
[NotStored(false)]
[ClassStorage("AwpAssessingProgram")]
[Caption("Программа проведения оценки обеспечения готовности к ОП")]
[AccessType(AccessType.@this)]
[View("AuditView", ["*"])]
[View("AwpAssessingProgramE", [
  "RegDate", "ActStatus", "ActNumber", "ChangeStatusDate", "OrganizationName",
  "OrganizationPhone", "OrganizationAddress", "ActNumberIndex",
  "Assignee", "ActType", "Organization", "Awp",
  "ResolutionNumber", "ResolutionDate", "ApprovedDate", "ResolutionDoc"
])]
[AssociatedDetailView("AwpAssessingProgramE", "Schedules", "AwpAssessingProgramEvaluationScheduleE", true, "", "", true, [""])]
[AssociatedDetailView("AwpAssessingProgramE", "Objects", "AwpAssessingProgramObjectE", true, "", "", true, [""])]
[View("AwpAssessingProgramL", [
  "RegDate", "ActStatus", "ActNumber", "ChangeStatusDate", "OrganizationName",
  "OrganizationPhone", "OrganizationAddress", "ActNumberIndex",
  "Assignee", "ActType", "Organization", "Awp",
  "ResolutionNumber", "ResolutionDate", "ApprovedDate", "ResolutionDoc"
])]
[BusinessServer(typeof(AwpAssessingProgramBS), DataServiceObjectEvents.OnAllEvents)]
public class AwpAssessingProgram : BaseAct, IWAwp
{
  public AwpAssessingProgram()
  {
    Schedules = new(this);
    Objects = new(this);
    ActSigns = new(this);
  }
  
  [StrLen(10)]
  public virtual string? ResolutionNumber { get; set; }

  public virtual DateTime? ResolutionDate { get; set; }

  public virtual DateTime? ApprovedDate { get; set; }

  public virtual string? ResolutionDoc { get; set; }

  [NotNull]
  [PropertyStorage(["Awp"])]
  public virtual Awp? Awp { get; set; }

  [PropertyStorage(["Committee"])]
  public virtual AdmissionCommittee? Committee { get; set; }

  public virtual DetailArray<AwpAssessingProgramEvaluationSchedule, 
    AwpAssessingProgram> Schedules { get; set; }

  public virtual DetailArray<AwpAssessingProgramObject, 
    AwpAssessingProgram> Objects { get; set; }

  public virtual DetailArray<AwpAssessingProgramSign,
    AwpAssessingProgram> ActSigns { get; set; }

    /// <summary>
    /// Class views container.
    /// </summary>
    public new class Views
  {
    public static readonly Tmobis.Core.Entities.EntityView AwpAssessingProgramLEntityView =
      new(nameof(AwpAssessingProgramL));

    /// <summary>
    /// "AuditView" view.
    /// </summary>
    public static View AuditView
    => Information.GetView("AuditView", typeof(AwpAssessingProgram));

    // /// <summary>
    // /// "ActTypeE" view.
    // /// </summary>
    // public static View ActTypeE
    // => Information.GetView("ActTypeE", typeof(ActType));

    /// <summary>
    /// "AwpAssessingProgramL" view.
    /// </summary>
    public static View AwpAssessingProgramL
    => Information.GetView("AwpAssessingProgramL", typeof(AwpAssessingProgram));
  }

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
