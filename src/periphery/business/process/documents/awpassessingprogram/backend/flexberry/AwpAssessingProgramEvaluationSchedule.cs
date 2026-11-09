namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using Tmobis.Business.Common.Address;
using Tmobis.Frameworks.Flexberry.Entities;

[AutoAltered]
[NotStored(false)]
[ClassStorage("AwpAssessingProgramEvaluationSchedule")]
[AccessType(AccessType.@this)]
[View("AuditView", ["*"])]
[View("AwpAssessingProgramEvaluationScheduleE", [
  "Program", "StartDate", "FinishDate", "ObjectsCount",
  "ObjectFunction"
])]
[BusinessServer(typeof(AwpAssessingProgramEvaluationScheduleAccessBS), DataServiceObjectEvents.OnAllEvents)]
public class AwpAssessingProgramEvaluationSchedule : FlexberryEntity
{
  public virtual int? ObjectsCount { get; set; }

  public virtual DateTime? StartDate { get; set; }

  public virtual DateTime? FinishDate { get; set; }

  [NotNull]
  [PropertyStorage(["ObjectFunction"])]
  public virtual ObjectFunction? ObjectFunction { get; set; }
  
  [NotNull]
  [Agregator]
  [PropertyStorage(["Program"])]
  public virtual AwpAssessingProgram? Program { get; set; }

  /// <summary>
  /// Class views container.
  /// </summary>
  public class Views
  {
    /// <summary>
    /// "AuditView" view.
    /// </summary>
    public static View AuditView
    => Information.GetView("AuditView",
      typeof(AwpAssessingProgramEvaluationSchedule));

    /// <summary>
    /// "AwpAssessingProgramEvaluationScheduleE" view.
    /// </summary>
    public static View EvaluationScheduleD
    => Information.GetView("AwpAssessingProgramEvaluationScheduleE",
      typeof(AwpAssessingProgramEvaluationSchedule));
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
