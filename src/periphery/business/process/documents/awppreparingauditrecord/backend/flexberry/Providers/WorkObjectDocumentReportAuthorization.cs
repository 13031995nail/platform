namespace Tmobis.Business.Process.Documents.Providers;

using Tmobis.Business.Common.Address;
using Tmobis.Core.Repositories;
using Tmobis.Reports.Models;

public enum WorkObjectDocumentReportType
{
    ReadinessAct,
    ReadinessPassport,
    EvaluationSheet,
    AssessingProgram,
}

public enum WorkObjectDocumentReportPolicy
{
    None,
    WorkObjectScope,
}

public sealed record WorkObjectDocumentReportRegistration(
    string TemplateName,
    WorkObjectDocumentReportType? DocumentType,
    WorkObjectDocumentReportPolicy Policy);

public static class WorkObjectDocumentReportRegistrations
{
    public static IReadOnlyList<WorkObjectDocumentReportRegistration> BuiltIn { get; } =
    [
        new("RedinessAct", WorkObjectDocumentReportType.ReadinessAct, WorkObjectDocumentReportPolicy.WorkObjectScope),
        new("ReadinessAct", WorkObjectDocumentReportType.ReadinessAct, WorkObjectDocumentReportPolicy.WorkObjectScope),
        new("rediness-act", WorkObjectDocumentReportType.ReadinessAct, WorkObjectDocumentReportPolicy.WorkObjectScope),
        new("AwpReadinessPassport", WorkObjectDocumentReportType.ReadinessPassport, WorkObjectDocumentReportPolicy.WorkObjectScope),
        new("EvaluationSheet", WorkObjectDocumentReportType.EvaluationSheet, WorkObjectDocumentReportPolicy.WorkObjectScope),
        new("AwpAssessingProgram", WorkObjectDocumentReportType.AssessingProgram, WorkObjectDocumentReportPolicy.WorkObjectScope),
    ];
}

/// <summary>
/// Проверка доступа к объектам документа перед формированием печатной формы.
/// </summary>
public sealed class WorkObjectDocumentReportAuthorization(
    IRepository repository,
    IWorkObjectAccessProvider accessProvider,
    EvaluationSheetPermissionProvider permissionProvider,
    IEnumerable<WorkObjectDocumentReportRegistration>? registrations = null) : IReportAuthorizationProvider
{
    private readonly IReadOnlyDictionary<string, WorkObjectDocumentReportRegistration> registrationsByTemplate =
        CreateRegistrationMap(registrations ?? WorkObjectDocumentReportRegistrations.BuiltIn);

    public bool Supports(ReportRunParams runParams, string? subTypeId, string? filialId)
    {
        ArgumentNullException.ThrowIfNull(runParams);

        return FindRegistrations(runParams, subTypeId, filialId).Count > 0;
    }

    public async Task EnsureCanRun(ReportRunParams runParams, string? subTypeId, string? filialId)
    {
        var policies = FindRegistrations(runParams, subTypeId, filialId);
        if (policies.Count == 0)
        {
            throw new InvalidOperationException("Провайдер вызван для незарегистрированной печатной формы.");
        }

        var policyKinds = policies
            .Select(registration => (registration.DocumentType, registration.Policy))
            .Distinct()
            .ToList();
        if (policyKinds.Count != 1)
        {
            throw new UnauthorizedAccessException("Для печатной формы зарегистрированы конфликтующие политики.");
        }

        var registration = policies[0];
        if (registration.Policy == WorkObjectDocumentReportPolicy.None)
        {
            return;
        }
        if (registration.Policy != WorkObjectDocumentReportPolicy.WorkObjectScope
            || !registration.DocumentType.HasValue)
        {
            throw new UnauthorizedAccessException("Политика печатной формы не поддерживается.");
        }

        var parameters = (runParams.Params ?? [])
            .Concat(runParams.ParamsList ?? [])
            .Where(parameter => parameter.Key.Equals("pk", StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (parameters.Count == 0
            || parameters.Any(parameter => !Guid.TryParse(parameter.Value, out var id) || id == Guid.Empty)
            || parameters.Select(parameter => Guid.Parse(parameter.Value)).Distinct().Count() != 1)
        {
            throw new UnauthorizedAccessException("Не задан корректный идентификатор документа.");
        }

        var documentId = Guid.Parse(parameters[0].Value);
        await EnsureDocumentScope(registration.DocumentType.Value, documentId);

        runParams.Params ??= [];
        foreach (var key in runParams.Params.Keys.Where(key => key.Equals("pk", StringComparison.OrdinalIgnoreCase)).ToList())
        {
            runParams.Params.Remove(key);
        }

        runParams.Params["pk"] = documentId.ToString("D");
    }

    private static IReadOnlyDictionary<string, WorkObjectDocumentReportRegistration> CreateRegistrationMap(
        IEnumerable<WorkObjectDocumentReportRegistration> registrations)
    {
        var map = new Dictionary<string, WorkObjectDocumentReportRegistration>(StringComparer.OrdinalIgnoreCase);
        foreach (var registration in registrations)
        {
            var templateName = NormalizeTemplateName(registration.TemplateName);
            if (string.IsNullOrWhiteSpace(templateName))
            {
                throw new ArgumentException("Имя шаблона не может быть пустым.", nameof(registrations));
            }

            if (!map.TryAdd(templateName, registration))
            {
                throw new ArgumentException($"Шаблон \"{templateName}\" зарегистрирован несколько раз.", nameof(registrations));
            }
        }

        return map;
    }

    private static string NormalizeTemplateName(string? templateName)
    {
        var name = (templateName ?? string.Empty).Replace('\\', '/').Split('/').Last();
        var extension = name.LastIndexOf('.');
        return (extension >= 0 ? name[..extension] : name).Trim();
    }

    private async Task EnsureDocumentScope(WorkObjectDocumentReportType documentType, Guid documentId)
    {
        Guid? awpId;
        List<Guid> objectIds;
        switch (documentType)
        {
            case WorkObjectDocumentReportType.ReadinessAct:
                var act = await repository.GetById<RedinessAct>(documentId);
                awpId = act?.Awp?.Id;
                objectIds = (await repository.Find<RedinessActObject>(item => item.Act.Id == documentId))
                    .Where(item => item.WorkObject != null).Select(item => item.WorkObject!.Id).ToList();
                if (act == null) throw new UnauthorizedAccessException("Документ недоступен.");
                break;
            case WorkObjectDocumentReportType.ReadinessPassport:
                var passport = await repository.GetById<AwpReadinessPassport>(documentId);
                awpId = passport?.Awp?.Id;
                objectIds = (await repository.Find<AwpReadinessPassportObject>(item => item.Act.Id == documentId))
                    .Where(item => item.WorkObject != null).Select(item => item.WorkObject!.Id).ToList();
                if (passport == null) throw new UnauthorizedAccessException("Документ недоступен.");
                break;
            case WorkObjectDocumentReportType.AssessingProgram:
                var program = await repository.GetById<AwpAssessingProgram>(documentId);
                awpId = program?.Awp?.Id;
                objectIds = (await repository.Find<AwpAssessingProgramObject>(item => item.Program.Id == documentId))
                    .Where(item => item.WorkObject != null).Select(item => item.WorkObject!.Id).ToList();
                if (program == null) throw new UnauthorizedAccessException("Документ недоступен.");
                break;
            case WorkObjectDocumentReportType.EvaluationSheet:
                var sheet = await repository.GetById<EvaluationSheet>(documentId,
                    EvaluationSheet.Views.EvaluationSheetPreparationEntityView);
                awpId = sheet?.Awp?.Id;
                objectIds = sheet?.WorkObject == null ? [] : [sheet.WorkObject.Id];
                break;
            default:
                throw new UnauthorizedAccessException("Тип документа для печатной формы не поддерживается.");
        }

        if (objectIds.Count == 0)
        {
            throw new UnauthorizedAccessException("Не заданы объекты документа.");
        }

        var tsoIds = await accessProvider.GetAccessibleWorkObjectIds(
            objectIds,
            WorkObjectRoleNames.Tso);
        var administrationIds = await accessProvider.GetAccessibleWorkObjectIds(
            objectIds,
            WorkObjectRoleNames.Administration);
        var allowedIds = tsoIds.Concat(administrationIds).ToHashSet();
        foreach (var objectId in objectIds.Distinct())
        {
            var canReviewEvaluationSheet = documentType == WorkObjectDocumentReportType.EvaluationSheet
                && (await permissionProvider.GetPermissions(objectId, awpId)).CanReview;
            if (!allowedIds.Contains(objectId) && !canReviewEvaluationSheet)
            {
                throw new UnauthorizedAccessException("Недостаточно полномочий для печати документа.");
            }
        }
    }

    private List<WorkObjectDocumentReportRegistration> FindRegistrations(
        ReportRunParams runParams,
        string? subTypeId,
        string? filialId)
    {
        var names = new[]
        {
            runParams.TemplateName,
            $"{runParams.TemplateName}{subTypeId}",
            $"{runParams.TemplateName}{subTypeId}_{filialId}",
        };

        return names
            .Select(NormalizeTemplateName)
            .Where(registrationsByTemplate.ContainsKey)
            .Select(name => registrationsByTemplate[name])
            .Distinct()
            .ToList();
    }
}
