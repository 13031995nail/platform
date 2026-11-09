namespace Tmobis.Business.Common.Contacts;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using System.Runtime.Serialization;
using Tmobis.Business.Core.Flexberry.Models;
using Tmobis.Business.Core.Models.Towns;
using Tmobis.Frameworks.Flexberry.Entities;

/// <summary>
/// Контрагенты.
/// </summary>
[ClassStorage("Organization")]
[AutoAltered]
[Caption("Контрагенты")]
[AccessType(AccessType.@this)]
[View("AuditView", [
    "Name", "Address", "LegalAddress", "Phone", "Email",
    "Acronym", "Actuality", "Town", "OrganizationType", "Hierarchy"
])]
[View("OrganizationE", [
    "Name", "Address", "LegalAddress", "Phone", "Email",
    "Acronym", "Actuality", "OrganizationType", "OrganizationType.Name",
    "Hierarchy", "Hierarchy.Name", "Hierarchy.Acronym"
])]
[View("OrganizationL", [
    "Name", "LegalAddress", "Phone", "OrganizationType"
])]
[BusinessServer(typeof(OrganizationBS), DataServiceObjectEvents.OnAllEvents)]
public class Organization :
  OrganizationBase,
  IOrganization,
  IFlexberryWTown
{
    [NotNull]
    [PropertyStorage(["Town"])]
    public virtual Core.Flexberry.Models.Town Town { get; set; }

    [NotNull]
    [PropertyStorage(["OrganizationType"])]
    public virtual OrganizationType OrganizationType { get; set; }

    [PropertyStorage(["Hierarchy"])]
    public virtual Organization? Hierarchy { get; set; }

    [StrLen(150)]
    public virtual string? HierarchyName { get; set; }

    [NotStored]
    [IgnoreDataMember]
    public Guid TownId { get => Town.Id; set {} }

    [NotStored]
    [IgnoreDataMember]
    ITown IWTown.Town
    {
        get => Town;
        set
        {
            if (value is Core.Flexberry.Models.Town town)
            {
                Town = town;
            }
        }
    }

    [NotStored]
    [IgnoreDataMember]
    IOrganizationBase? IOrganization.Hierarchy
    {
        get => Hierarchy;
        set
        {
            if (value is Organization org)
            {
                Hierarchy = org;
            }
        }
    }

    /// <summary>
    /// Class views container.
    /// </summary>
    public class Views
    {
        public static readonly Tmobis.Core.Entities.EntityView OrganizationEEntityView = new(nameof(OrganizationE));
        /// <summary>
        /// "AuditView" view.
        /// </summary>
        public static View AuditView
          => Information.GetView("AuditView", typeof(Organization));

        /// <summary>
        /// "OrganizationE" view.
        /// </summary>
        public static View OrganizationE
          => Information.GetView("OrganizationE", typeof(Organization));

        /// <summary>
        /// "OrganizationL" view.
        /// </summary>
        public static View OrganizationL
          => Information.GetView("OrganizationL", typeof(Organization));
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
