namespace Tmobis.Business.Common.Resources;

using ICSSoft.STORMNET;
using Tmobis.Frameworks.Flexberry.Entities;
using Tmobis.Business.Common.Contacts;

/// <summary>
/// Требование проверки.
/// </summary>
[AutoAltered]
[NotStored(false)]
[Caption("Требование проверки")]
[AccessType(AccessType.@this)]
[View("AuditView", [
    "CreateTime", "Creator", "EditTime", "Editor", "Code", "Name", "IsCalculated",
    "OrganizationType", "ShortReqName", "ReqNumber", "ShortIndicatorName", "IndicatorName",
    "SupportingDocName", "Weight","Actuality", "IsOptional", "OwnerlessObjects"
])]
[View("VerificationRequirementE", [
    "Code", "Name", "IsCalculated", "OrganizationType", "ShortReqName", "ReqNumber",
    "ShortIndicatorName", "IndicatorName", "SupportingDocName","Weight","Actuality",
    "IsOptional", "OwnerlessObjects"
], Hidden = new string[] { })]
[View("VerificationRequirementL", [
    "Code", "Name", "IsCalculated", "OrganizationType", "ShortReqName", "ReqNumber",
    "ShortIndicatorName", "IndicatorName", "SupportingDocName", "Weight", "Actuality",
    "IsOptional", "OwnerlessObjects"
], Hidden = new string[] { })]
[ClassStorage("VerificationRequierement")]
public class VerificationRequierement : FlexberryDictionary, IVerificationRequierement
{
  public virtual bool IsCalculated { get; set; } = false;

  [NotNull]
  [PropertyStorage(["OrganizationType"])]
  public virtual OrganizationType OrganizationType { get; set; }

  [StrLen(2000)]
  public virtual string ShortReqName { get; set; }

  [NotNull]
  [StrLen(10)]
  public virtual string ReqNumber { get; set; }

  [StrLen(200)]
  public virtual string ShortIndicatorName { get; set; }

  [NotNull]
  [StrLen(2000)]
  public virtual string IndicatorName { get; set; }

  [StrLen(2000)]
  public virtual string SupportingDocName { get; set; }

  public virtual System.Decimal? Weight { get; set; }

  public virtual bool IsOptional { get; set; }

  public virtual bool OwnerlessObjects { get; set; }

  public class Keys
  {
    public static Guid Empty { get; set; } = Guid.Parse("a683b9ce-3367-4fac-8604-6362a51f3204");
  }

  /// <summary>
  /// Class views container.
  /// </summary>
  public class Views
  {

    /// <summary>
    /// "AuditView" view.
    /// </summary>
    public static View AuditView
      => Information.GetView("AuditView", typeof(VerificationRequierement));

    /// <summary>
    /// "VerificationRequierementE" view.
    /// </summary>
    public static View VerificationRequierementE
              => Information.GetView("VerificationRequierementE", typeof(VerificationRequierement));

    /// <summary>
    /// "VerificationRequierementL" view.
    /// </summary>
    public static View VerificationRequierementL
              => Information.GetView("VerificationRequierementL", typeof(VerificationRequierement));
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

