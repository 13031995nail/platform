namespace Tmobis.Business.Common.Contacts;

using ICSSoft.STORMNET;
using Tmobis.Frameworks.Flexberry.Entities;

/// <summary>
/// Тип контрагента.
/// </summary>
[AutoAltered]
[NotStored(false)]
[Caption("Тип контрагента")]
[AccessType(AccessType.@this)]
[View("AuditView", [
    "Creator", "CreateTime", "Editor", "EditTime", "Code", "Name", "ShortName", "Actuality"
])]
[View("OrganizationTypeE", [
    "Code", "Name", "ShortName", "Actuality"
])]
[View("OrganizationTypeL", [
    "Code", "Name", "ShortName", "Actuality"
])]
[ClassStorage("OrganizationType")]
public class OrganizationType : FlexberryDictionary
{
  [StrLen(250)]
  public string ShortName { get; set; }

  public class Keys
  {
    public static Guid Other { get; set; } = Guid.Parse("1fb68a1f-7533-4822-a312-29d978168327");
    public static Guid Municipality { get; set; } = Guid.Parse("7909c321-f77c-4bdf-9d4d-e618fac50c81");
    public static Guid NotTco { get; set; } = Guid.Parse("3859980c-562d-4ef3-b716-e3dc72dc6bbe");
    public static Guid Tco { get; set; } = Guid.Parse("44cf13f0-5b06-4509-a11d-1c05c9c39e80");
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
      => Information.GetView("AuditView", typeof(OrganizationType));

    /// <summary>
    /// "OrganizationTypeE" view.
    /// </summary>
    public static View OrganizationTypeE
              => Information.GetView("OrganizationTypeE", typeof(OrganizationType));

    /// <summary>
    /// "OrganizationTypeL" view.
    /// </summary>
    public static View OrganizationTypeL
              => Information.GetView("OrganizationTypeL", typeof(OrganizationType));

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
