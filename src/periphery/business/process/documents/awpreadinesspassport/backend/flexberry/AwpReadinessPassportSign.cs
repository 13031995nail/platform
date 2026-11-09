namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using Tmobis.Business.Process.Documents.Flexberry;

/// <summary>
/// Подпись паспорта обеспечения готовности.
/// </summary>
[AutoAltered]
[NotStored(false)]
[ClassStorage("AwpReadinessPassportSign")]
[Caption("Подпись паспорта обеспечения готовности")]
[AccessType(AccessType.@this)]
[View("AuditView", ["*"])]
[View("AwpReadinessPassportSignD", [
  "Signature", "SignDate", "SignRole", "SignType", "Employee",
  "Fio", "Address", "Email",
  "Phone", "Passport", "PositionName", "PartnerName", "Act"
])]
[BusinessServer(typeof(AwpReadinessPassportSignBS), DataServiceObjectEvents.OnAllEvents)]
public class AwpReadinessPassportSign : ActSign
{
  [NotNull]
  [Agregator]
  [PropertyStorage(["Act"])]
  public virtual AwpReadinessPassport? Act { get; set; }

  /// <summary>
  /// Class views container.
  /// </summary>
  public new class Views
  {
    /// <summary>
    /// "AuditView" view.
    /// </summary>
    public static View AuditView
    => Information.GetView("AuditView", typeof(AwpReadinessPassportSign));

    /// <summary>
    /// "AwpReadinessPassportSignD" view.
    /// </summary>
    public static View AwpReadinessPassportSignD
    => Information.GetView("AwpReadinessPassportSignD", typeof(AwpReadinessPassportSign));
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
