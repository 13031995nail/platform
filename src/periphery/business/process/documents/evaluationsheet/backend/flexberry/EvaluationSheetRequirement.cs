extern alias verificationflexberry;

namespace Tmobis.Business.Process.Documents;

using System;
using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using Tmobis.Core.Entities;
using Tmobis.Frameworks.Flexberry.Entities;
using VerificationRequierement = verificationflexberry::Tmobis.Business.Common.Resources.VerificationRequierement;

/// <summary>
/// Детайл требований оценочного листа.
/// </summary>
[AutoAltered]
[NotStored(false)]
[ClassStorage("EvaluationSheetRequirement")]
[AccessType(AccessType.@this)]
[View("AuditView", ["*"])]
[View("EvaluationSheetRequirementE", [
  "ReqOrder", "ReqNumber", "ShortReqName", "IndicatorName", "Weight",
  "EvaluationSheet", "VerificationRequirement", "SupportingDocName",
  "SupportingDocFile", "SortOrder", "CalculatedReq", "IsCalculated", "IsCompletedReq",
  "ProblemComment", "Deadline", "IsOptional", "IsVerified", "HasEnteredValue"
])]
[View("EvaluationSheetRequirementL", [
  "ReqOrder", "ReqNumber", "ShortReqName", "IndicatorName", "Weight",
  "EvaluationSheet", "VerificationRequirement", "SupportingDocName",
  "SupportingDocFile", "SortOrder", "CalculatedReq", "IsCalculated", "IsCompletedReq",
  "ProblemComment", "Deadline", "IsOptional", "IsVerified", "HasEnteredValue"
])]
[View("EvaluationSheetRequirementWorkspace", [
  "EvaluationSheet", "ReqOrder", "ReqNumber", "ShortReqName", "SupportingDocName",
  "SupportingDocFile", "SortOrder", "IsCalculated", "IsCompletedReq", "ProblemComment",
  "Deadline", "IsOptional", "IsVerified", "HasEnteredValue"
])]
[BusinessServer(typeof(EvaluationSheetRequirementBS), DataServiceObjectEvents.OnAllEvents)]
[View("EvaluationSheetRequirementAccess", [
  "EvaluationSheet", "EvaluationSheet.WorkObject", "EvaluationSheet.Awp"
])]
[View("EvaluationSheetRequirementCompletion", [
  "EvaluationSheet", "ReqOrder", "SupportingDocName", "SupportingDocFile",
  "IsCalculated", "IsOptional", "HasEnteredValue"
])]
public class EvaluationSheetRequirement : FlexberryEntity
{
    [NotNull]
    [Agregator]
    [PropertyStorage(["EvaluationSheet"])]
    public virtual EvaluationSheet EvaluationSheet { get; set; }

    [NotNull]
    [PropertyStorage(["VerificationRequirement"])]
    public virtual VerificationRequierement VerificationRequirement { get; set; }

    public virtual int? ReqOrder { get; set; }

    [StrLen(10)]
    public virtual string? ReqNumber { get; set; }

    [StrLen(250)]
    public virtual string? ShortReqName { get; set; }

    [StrLen(250)]
    public virtual string? IndicatorName { get; set; }

    public virtual decimal? Weight { get; set; }
    [StrLen(250)]
    public virtual string? SupportingDocName { get; set; }

    [StrLen(1024)]
    public virtual string? SupportingDocFile { get; set; }

    public virtual int SortOrder { get; set; }

    public virtual decimal CalculatedReq { get; set; } = 0m;

    public virtual bool HasEnteredValue { get; set; }

    public virtual bool IsCalculated { get; set; }

    public virtual bool IsCompletedReq { get; set; }

    [StrLen(4000)]
    public virtual string? ProblemComment { get; set; }

    public virtual DateTime? Deadline { get; set; }

    public virtual bool IsOptional { get; set; }

    public virtual bool IsVerified { get; set; }

    /// <summary>
    /// Class views container.
    /// </summary>
    public class Views
    {
        public static readonly EntityView EvaluationSheetRequirementEEntityView = new(nameof(EvaluationSheetRequirementE));
        public static readonly EntityView EvaluationSheetRequirementWorkspaceEntityView = new(nameof(EvaluationSheetRequirementWorkspace));
        public static readonly EntityView EvaluationSheetRequirementAccessEntityView = new(nameof(EvaluationSheetRequirementAccess));
        public static readonly EntityView EvaluationSheetRequirementCompletionEntityView = new(nameof(EvaluationSheetRequirementCompletion));

        public static View EvaluationSheetRequirementCompletion
            => Information.GetView("EvaluationSheetRequirementCompletion", typeof(EvaluationSheetRequirement));

        public static View EvaluationSheetRequirementAccess
            => Information.GetView("EvaluationSheetRequirementAccess", typeof(EvaluationSheetRequirement));
        /// <summary>
        /// "AuditView" view.
        /// </summary>
        public static View AuditView
            => Information.GetView("AuditView", typeof(EvaluationSheetRequirement));

        /// <summary>
        /// "EvaluationSheetRequirementE" view.
        /// </summary>
        public static View EvaluationSheetRequirementE
            => Information.GetView("EvaluationSheetRequirementE", typeof(EvaluationSheetRequirement));

        /// <summary>
        /// "EvaluationSheetRequirementL" view.
        /// </summary>
        public static View EvaluationSheetRequirementL
            => Information.GetView("EvaluationSheetRequirementL", typeof(EvaluationSheetRequirement));

        public static View EvaluationSheetRequirementWorkspace
            => Information.GetView("EvaluationSheetRequirementWorkspace", typeof(EvaluationSheetRequirement));
    }

    /// <summary>
    /// Audit settings.
    /// </summary>
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
