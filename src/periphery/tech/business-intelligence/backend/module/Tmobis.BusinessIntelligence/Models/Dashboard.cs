namespace Tmobis.BusinessIntelligence;
using ICSSoft.STORMNET;
using Tmobis.Frameworks.Flexberry.Entities;

[AutoAltered()]
[AccessType(AccessType.none)]
[View("AuditView", new string[] {
    "CreateTime",
    "Creator",
    "Editor",
    "EditTime",
    "TmobisDashboardId",
    "MetabaseDashboardId",
    "Note",
})]
public class Dashboard
: FlexberryDictionary, IDashboard
{
    public string? TmobisDashboardId { get; set; }

    public string? MetabaseDashboardId  { get; set; }

    public string? Note { get; set; }

    public class Views
    {

      /// <summary>
      /// "AuditView" view.
      /// </summary>
      public static View AuditView
      {
        get
        {
          return Information.GetView("AuditView", typeof(Dashboard));
        }
      }
    }

    /// <summary>
    /// Audit class settings.
    /// </summary>
    public class AuditSettings
    {

      /// <summary>
      /// Включён ли аудит для класса.
      /// </summary>
      public static bool AuditEnabled = true;

      /// <summary>
      /// Использовать имя представления для аудита по умолчанию.
      /// </summary>
      public static bool UseDefaultView = false;

      /// <summary>
      /// Включён ли аудит операции чтения.
      /// </summary>
      public static bool SelectAudit = false;

      /// <summary>
      /// Имя представления для аудирования операции чтения.
      /// </summary>
      public static string SelectAuditViewName = "AuditView";

      /// <summary>
      /// Включён ли аудит операции создания.
      /// </summary>
      public static bool InsertAudit = true;

      /// <summary>
      /// Имя представления для аудирования операции создания.
      /// </summary>
      public static string InsertAuditViewName = "AuditView";

      /// <summary>
      /// Включён ли аудит операции изменения.
      /// </summary>
      public static bool UpdateAudit = true;

      /// <summary>
      /// Имя представления для аудирования операции изменения.
      /// </summary>
      public static string UpdateAuditViewName = "AuditView";

      /// <summary>
      /// Включён ли аудит операции удаления.
      /// </summary>
      public static bool DeleteAudit = true;

      /// <summary>
      /// Имя представления для аудирования операции удаления.
      /// </summary>
      public static string DeleteAuditViewName = "AuditView";

      /// <summary>
      /// Путь к форме просмотра результатов аудита.
      /// </summary>
      public static string FormUrl = "";

      /// <summary>
      /// Режим записи данных аудита (синхронный или асинхронный).
      /// </summary>
      public static ICSSoft.STORMNET.Business.Audit.Objects.tWriteMode WriteMode = ICSSoft.STORMNET.Business.Audit.Objects.tWriteMode.Synchronous;

      /// <summary>
      /// Максимальная длина сохраняемого значения поля (если 0, то строка обрезаться не будет).
      /// </summary>
      public static int PrunningLength = 0;

      /// <summary>
      /// Показывать ли пользователям в изменениях первичные ключи.
      /// </summary>
      public static bool ShowPrimaryKey = false;

      /// <summary>
      /// Сохранять ли старое значение.
      /// </summary>
      public static bool KeepOldValue = true;

      /// <summary>
      /// Сжимать ли сохраняемые значения.
      /// </summary>
      public static bool Compress = false;

      /// <summary>
      /// Сохранять ли все значения атрибутов, а не только изменяемые.
      /// </summary>
      public static bool KeepAllValues = false;
    }
}
