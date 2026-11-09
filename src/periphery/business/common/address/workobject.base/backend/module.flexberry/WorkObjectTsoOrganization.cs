namespace Tmobis.Business.Common.Address;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using Tmobis.Business.Common.Contacts;
using Tmobis.Frameworks.Flexberry.Entities;
using Tmobis.Core.Entities;

/// <summary>
/// Связь объекта с теплоснабжающей организацией.
/// </summary>
[AutoAltered]
[NotStored(false)]
[ClassStorage("WorkObjectTsoOrganization")]
[AccessType(AccessType.@this)]
[View("AuditView", ["WorkObject", "Organization"])]
[View("WorkObjectTsoOrganizationE", [
    "WorkObject", "WorkObject.Name", "Organization", "Organization.Name"
])]
[View("WorkObjectTsoOrganizationL", [
    "WorkObject", "WorkObject.Name", "Organization", "Organization.Name"
])]
[BusinessServer(typeof(WorkObjectTsoOrganizationBS), DataServiceObjectEvents.OnAllEvents)]
public class WorkObjectTsoOrganization : FlexberryEntity
{
    [NotNull]
    [PropertyStorage(["WorkObject"])]
    public virtual WorkObject WorkObject { get; set; } = null!;

    [NotNull]
    [PropertyStorage(["Organization"])]
    public virtual Organization Organization { get; set; } = null!;

    public class Views
    {
        public static readonly EntityView WorkObjectTsoOrganizationLEntityView = new(nameof(WorkObjectTsoOrganizationL));

        public static View AuditView
            => Information.GetView("AuditView", typeof(WorkObjectTsoOrganization));

        public static View WorkObjectTsoOrganizationE
            => Information.GetView("WorkObjectTsoOrganizationE", typeof(WorkObjectTsoOrganization));

        public static View WorkObjectTsoOrganizationL
            => Information.GetView("WorkObjectTsoOrganizationL", typeof(WorkObjectTsoOrganization));
    }

    public class AuditSettings
    {
        public static readonly bool AuditEnabled = true;
        public static readonly bool UseDefaultView = false;
        public static readonly bool SelectAudit = false;
        public static readonly string SelectAuditViewName = "AuditView";
        public static readonly bool InsertAudit = true;
        public static readonly string InsertAuditViewName = "AuditView";
        public static readonly bool UpdateAudit = true;
        public static readonly string UpdateAuditViewName = "AuditView";
        public static readonly bool DeleteAudit = true;
        public static readonly string DeleteAuditViewName = "AuditView";
        public static readonly string FormUrl = "";
        public static readonly ICSSoft.STORMNET.Business.Audit.Objects.tWriteMode WriteMode
            = ICSSoft.STORMNET.Business.Audit.Objects.tWriteMode.Synchronous;
        public static readonly int PrunningLength = 0;
        public static readonly bool ShowPrimaryKey = false;
        public static readonly bool KeepOldValue = true;
        public static readonly bool Compress = false;
        public static readonly bool KeepAllValues = false;
    }
}
