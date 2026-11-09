namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Frameworks.Flexberry.Entities;
using Tmobis.Core.Entities;

/// <summary>
/// Оценочный лист.
/// </summary>
[AutoAltered]
[NotStored(false)]
[ClassStorage("EvaluationSheet")]
[Caption("Оценочный лист")]
[AccessType(AccessType.@this)]
[View("AuditView", ["*"])]
[View("EvaluationSheetE", [
  "RegDate", "ActStatus", "ActNumber", "ChangeStatusDate", "OrganizationName",
  "OrganizationPhone", "OrganizationAddress", "ActNumberIndex", "Assignee",
  "ActType", "Organization", "WorkObject", "Awp", "Representative", "ObjectReadinessLevel",
  "ObjectReadinessIndex", "IsVerifiedTSO", "OwnerlessObjects", "ReviewRequestedAt", "ReviewStartedAt"
])]
[AssociatedDetailView("EvaluationSheetE", "Requirements", "EvaluationSheetRequirementE", true, "", "", true, [""])]
[View("EvaluationSheetL", [
  "RegDate", "ActStatus", "ActNumber", "ChangeStatusDate", "OrganizationName",
  "OrganizationPhone", "OrganizationAddress", "ActNumberIndex", "Assignee",
  "ActType", "Organization", "WorkObject", "Awp", "ObjectReadinessLevel", "ObjectReadinessIndex"
])]
[View("EvaluationSheetPreparation", [
  "RegDate", "ActStatus", "ChangeStatusDate", "WorkObject", "Awp",
  "ObjectReadinessLevel", "ObjectReadinessIndex", "IsVerifiedTSO", "ReviewRequestedAt", "ReviewStartedAt"
])]
[BusinessServer(typeof(EvaluationSheetBS), DataServiceObjectEvents.OnAllEvents)]
public class EvaluationSheet : BaseAct
{
    public EvaluationSheet()
    {
        Requirements = new(this);
        ActSigns = new(this);
    }

    [NotNull]
    [PropertyStorage(["WorkObject"])]
    public virtual WorkObject WorkObject { get; set; }

    [NotNull]
    [PropertyStorage(["Awp"])]
    public virtual Awp Awp { get; set; }

    [StrLen(250)]
    public virtual string? Representative { get; set; }

    public virtual decimal ObjectReadinessIndex { get; set; } = 0m;

    public virtual LevelReadiness ObjectReadinessLevel { get; set; } = LevelReadiness.NotReady;

    public virtual bool IsVerifiedTSO { get; set; }

    public virtual DateTime? ReviewRequestedAt { get; set; }

    public virtual DateTime? ReviewStartedAt { get; set; }

    public virtual bool OwnerlessObjects { get; set; }

    public virtual DetailArray<EvaluationSheetRequirement, EvaluationSheet> Requirements { get; set; }

    public virtual DetailArray<EvaluationSheetSign, EvaluationSheet> ActSigns { get; set; }

    /// <summary>
    /// Class views container.
    /// </summary>
    public new class Views
    {
        public static readonly EntityView EvaluationSheetEEntityView = new(nameof(EvaluationSheetE));
        public static readonly EntityView EvaluationSheetPreparationEntityView = new(nameof(EvaluationSheetPreparation));

        /// <summary>
        /// "AuditView" view.
        /// </summary>
        public static View AuditView
            => Information.GetView("AuditView", typeof(EvaluationSheet));

        /// <summary>
        /// "EvaluationSheetE" view.
        /// </summary>
        public static View EvaluationSheetE
            => Information.GetView("EvaluationSheetE", typeof(EvaluationSheet));

        /// <summary>
        /// "EvaluationSheetL" view.
        /// </summary>
        public static View EvaluationSheetL
            => Information.GetView("EvaluationSheetL", typeof(EvaluationSheet));

        public static View EvaluationSheetPreparation
            => Information.GetView("EvaluationSheetPreparation", typeof(EvaluationSheet));
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
