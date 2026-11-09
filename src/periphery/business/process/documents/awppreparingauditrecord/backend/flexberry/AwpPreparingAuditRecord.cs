namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Resources;
using Tmobis.Frameworks.Flexberry.Entities;

/// <summary>
/// Журнал контроля.
/// </summary>
[AutoAltered]
[NotStored(false)]
[ClassStorage("AwpPreparingAuditRecord")]
[Caption("Журнал контроля")]
[AccessType(AccessType.@this)]
[View("AuditView", ["*"])]
[View("AwpPreparingAuditRecordE", [
  "WorkObject", "WorkObject.Organization", "WorkObject.Organization.Hierarchy", "WorkObject.Organization.Hierarchy.Name", "WorkObject.Organization.Name", "WorkObject.Name", "WorkObject.ObjectFunction", "WorkObject.ObjectFunction.Name",
  "EvaluationSheet", "EvaluationSheetDate", "ReadinessAct", "ReadinessActDate", "AwpReadinessPassport", "PassportDate", 
  "IndexReadiness", "IndexPartnerReadiness", "PartnerReadiness", "Awp"
])]
[View("AwpPreparingAuditRecordL", [
  "WorkObject", "WorkObject.Organization", "WorkObject.Organization.Hierarchy", "WorkObject.Organization.Hierarchy.Name", "WorkObject.Organization.Name", "WorkObject.Name", "WorkObject.ObjectFunction", "WorkObject.ObjectFunction.Name",
  "EvaluationSheet", "EvaluationSheetDate", "ReadinessAct", "ReadinessActDate", "AwpReadinessPassport", "PassportDate",
  "IndexReadiness", "IndexPartnerReadiness", "PartnerReadiness", "Awp"
])]
[BusinessServer(typeof(AwpPreparingAuditRecordBS), DataServiceObjectEvents.OnAllEvents)]
public class AwpPreparingAuditRecord : FlexberryEntity, IWAwp
{
  public AwpPreparingAuditRecord()
  {
  }
    
  [PropertyStorage(["AwpReadinessPassport"])]
  public virtual AwpReadinessPassport? AwpReadinessPassport { get; set; }
  public virtual DateTime? PassportDate { get; set; }

  [PropertyStorage(["EvaluationSheet"])]
  public virtual EvaluationSheet? EvaluationSheet { get; set; }
  public virtual DateTime? EvaluationSheetDate { get; set; }
  public virtual bool HasEvaluationSheetProblem { get; set; }
  [NotStored]
  public virtual string EvaluationSheetString { get => EvaluationSheetDate.HasValue
    ? EvaluationSheetDate.Value.ToString("dd.MM.yyyy") + (HasEvaluationSheetProblem ? " / Выявлены замечания" : string.Empty)
    : string.Empty;
    set { }
  }

  [PropertyStorage(["ReadinessAct"])]
  public virtual RedinessAct? ReadinessAct { get; set; }
  public virtual DateTime? ReadinessActDate { get; set; }

  public virtual decimal? IndexReadiness { get; set; }
  public virtual decimal? IndexPartnerReadiness { get; set; }
  public virtual PartnerReadinessLevel PartnerReadiness { get; set; }

  [NotNull]
  [PropertyStorage(["Awp"])]
  public virtual Awp? Awp { get; set; }

  [NotNull]
  [PropertyStorage(["WorkObject"])]
  public virtual WorkObject? WorkObject { get; set; }

  /// <summary>
  /// Class views container.
  /// </summary>
  public new class Views
  {
    /// <summary>
    /// "AuditView" view.
    /// </summary>
    public static View AuditView
    => Information.GetView("AuditView", typeof(AwpPreparingAuditRecord));


    /// <summary>
    /// "AwpPreparingAuditRecordE" view.
    /// </summary>
    public static View AwpPreparingAuditRecordE
    => Information.GetView("AwpPreparingAuditRecordE", typeof(AwpPreparingAuditRecord));

    /// <summary>
    /// "AwpPreparingAuditRecordL" view.
    /// </summary>
    public static View AwpPreparingAuditRecordL
    => Information.GetView("AwpPreparingAuditRecordL", typeof(AwpPreparingAuditRecord));
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
