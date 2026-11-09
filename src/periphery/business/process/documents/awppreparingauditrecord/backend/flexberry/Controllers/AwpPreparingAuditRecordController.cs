namespace Tmobis.Business.Process.Documents.Controllers;

using ICSSoft.STORMNET;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Common.Address;
using Common.Personnel;
using Common.Resources;
using Documents;
using Providers;
using Tmobis.Core.Repositories;
using UnauthorizedAccessException = System.UnauthorizedAccessException;

[ApiController]
[Authorize]
[Route("api/AwpPreparingAuditRecord")]
public class AwpPreparingAuditRecordController(
    IRepository repository,
    IUnitOfWork unitOfWork,
    IAwpPreparingAuditRecordProvider awpPreparingAuditRecordProvider,
    AwpPreparingAuditRecordCommissionAccess commissionAccess,
    ILogger<AwpPreparingAuditRecordController> logger)
: ControllerBase
{
    /// <summary>
    /// Сформировать записи журнала контроля подготовки к ОЗП для заданных ОЗП и инспектора.
    /// </summary>
    /// <param name="awpId">Идентификатор ОЗП.</param>
    /// <param name="employeeId">Идентификатор инспектора.</param>
    [HttpGet("UpdateRecords")]
    public Task<ActionResult> UpdateRecords([FromQuery] Guid awpId, [FromQuery] Guid employeeId)
    {
        return UpdateRecordsV2(awpId, employeeId, null);
    }

    /// <summary>
    /// Сформировать записи журнала для заданных объектов теплопотребления.
    /// </summary>
    /// <param name="awpId">Идентификатор ОЗП.</param>
    /// <param name="workObjectId">Идентификатор объекта теплопотребления,.</param>
    [HttpGet("UpdateRecordsForObjects")]
    public Task<ActionResult> UpdateRecordsForObjects([FromQuery] Guid awpId, [FromQuery] Guid employeeId, [FromQuery] Guid workObjectId)
    {
        return UpdateRecordsV2(awpId, employeeId, workObjectId);
    }

    [NonAction]
    public async Task<ActionResult> UpdateRecordsV2(Guid awpId, Guid employeeId, Guid? workObjectId)
    {
        try
        {
            if (workObjectId.HasValue)
            {
                var (committee, workObject) = await commissionAccess.GetCommitteeForObject(
                    awpId, employeeId, workObjectId.Value);
                var availableObjects = await GetAvailableWorkObjects(awpId, committee, includeExisting: true);
                if (!availableObjects.Any(item => item.Id == workObject.Id))
                {
                    throw new UnauthorizedAccessException("Объект не включён в программу комиссии.");
                }
                var organizationId = committee.Organization.Id;
                await FillRecord(awpId, workObject, organizationId,
                    await GetPassport(awpId, organizationId), await GetReadinessAct(awpId, organizationId));
                await awpPreparingAuditRecordProvider.UpdatePartnerRecords(awpId, workObject.Organization.Id);
            }
            else
            {
                // сформировать журнал контроля полностью
                var committees = await commissionAccess.GetCommittees(awpId, employeeId);
                var batches = new List<(AdmissionCommittee Committee, List<WorkObject> Objects)>();
                foreach (var committee in committees)
                {
                    batches.Add((committee, await GetAvailableWorkObjects(awpId, committee)));
                }
                Dictionary<Guid, AwpReadinessPassport?> passportsDict = new Dictionary<Guid, AwpReadinessPassport?>();
                Dictionary<Guid, RedinessAct?> readinessActsDict = new Dictionary<Guid, RedinessAct?>();
                List<Guid> orgList = new List<Guid>();
                foreach (var (committee, objects) in batches)
                {
                    var organizationId = committee.Organization.Id;
                    foreach (var r in objects)
                    {
                        if (!passportsDict.ContainsKey(organizationId))
                        {
                            passportsDict.Add(organizationId, await GetPassport(awpId, organizationId));
                        }
                        if (!readinessActsDict.ContainsKey(organizationId))
                        {
                            readinessActsDict.Add(organizationId, await GetReadinessAct(awpId, organizationId));
                        }
                        await FillRecord(awpId, r, organizationId,
                            passportsDict[organizationId], readinessActsDict[organizationId]);
                        if (!orgList.Contains(r.Organization.Id))
                        {
                            await awpPreparingAuditRecordProvider.UpdatePartnerRecords(awpId, r.Organization.Id);
                            orgList.Add(r.Organization.Id);
                        }
                    }
                }
            }

            await unitOfWork.Commit();
            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            const string msg = "Ошибка при получении списка объектов";
            logger.LogError(ex, msg);

            return Problem(ex.Message ?? msg);
        }
    }

    private async Task<AwpReadinessPassport?> GetPassport(Guid awpId, Guid organizationId)
    {
        var passports = await repository.Find<AwpReadinessPassport>(x => x.Awp.Id == awpId && x.ActStatus.Id == IActStatus.Keys.Signed
            && x.Organization.Id == organizationId);
        if (passports.Count > 0)
        {
            return passports.OrderByDescending(x => x.ChangeStatusDate).First();
        }
        return null;
    }

    private async Task<RedinessAct?> GetReadinessAct(Guid awpId, Guid organizationId)
    {
        var readinessacts = await repository.Find<RedinessAct>(x => x.Awp.Id == awpId && x.ActStatus.Id == IActStatus.Keys.Signed
               && x.Organization.Id == organizationId);
        if (readinessacts.Count > 0)
        {
            return readinessacts.OrderByDescending(x => x.ChangeStatusDate).First();
        }
        return null;
    }

    /// <summary>
    /// Возвращает уникальные объекты по указанному AWP.
    /// </summary>
    [HttpGet]
    [Route("AvailableObjects")]
    public async Task<ActionResult> GetAvailableObjects(Guid awpId, Guid employeeId)
    {
        try
        {
            if (awpId == Guid.Empty)
            {
                const string msg = "Не задан идентификатор ОЗП.";
                LogService.LogError(msg);
                return Problem(msg);
            }

            if (employeeId == Guid.Empty)
            {
                const string msg = "Не задан идентификатор председателя.";
                LogService.LogError(msg);
                return Problem(msg);
            }
            var committees = await commissionAccess.GetCommittees(awpId, employeeId);
            var objects = new List<WorkObject>();
            foreach (var committee in committees)
            {
                objects.AddRange(await GetAvailableWorkObjects(awpId, committee));
            }

            var ids = objects.Select(r => r.Id).Distinct().ToList();

            return Ok(ids);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            const string msg = "Ошибка при получении списка объектов";
            logger.LogError(ex, msg);

            return Problem(msg);
        }
    }

    /// <summary>
    /// Получаются все актуальные объекты, организации которых принадлежат МО председателя И их назначение присутствует в программе МО на этот же ОЗП
    /// </summary>
    /// <param name="awpId"></param>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task<List<WorkObject>> GetAvailableWorkObjects(
        Guid awpId, AdmissionCommittee committee, bool includeExisting = false)
    {
        var programm = await repository.Find<AwpAssessingProgram>(x => x.Awp != null && x.Awp.Id == awpId
            && x.ActStatus.Id == IActStatus.Keys.Approved && x.Organization != null && x.Organization.Id == committee.Organization.Id
            && x.Committee != null && x.Committee.Id == committee.Id);
        if (programm.Count == 0 || programm.First().Schedules.Count == 0)
        {
            throw new Exception("Не найдена утвержденная программа для МО.");
        }
        var existRecords = includeExisting ? [] : await repository.Find<AwpPreparingAuditRecord>(x => x.Awp != null && x.Awp.Id == awpId
            && (x.WorkObject.Organization.Id == committee.Organization.Id
                || x.WorkObject.Organization.Hierarchy.Id == committee.Organization.Id));
        var objects = await repository.Find<WorkObject>(x => x.Actuality
            && (x.Organization.Id == committee.Organization.Id
                || x.Organization.Hierarchy != null && x.Organization.Hierarchy.Id == committee.Organization.Id));
        if (objects.Count > 0)
        {
            if (existRecords.Count > 0)
            {
                var existObjects = existRecords.Select(x => x.WorkObject.Id);
                objects = objects.Where(x => !existObjects.Contains(x.Id)).ToList();
            }
            var objFunctions = programm.First().Schedules.Select(x => x.ObjectFunction.Id);
            objects = objects.Where(x => objFunctions.Contains(x.ObjectFunction.Id)).ToList();
        }
        return objects;
    }

    private async Task FillRecord(Guid awpId, WorkObject workObject, Guid? organizationId, AwpReadinessPassport? passport, RedinessAct? readinessAct)
    {
        var record = repository.MakeEntity<AwpPreparingAuditRecord>();
        record.Awp = await repository.GetById<Awp>(awpId);
        record.WorkObject = workObject;
        if (organizationId.HasValue)
        {
            if (passport != null)
            {
                var objectsInPassport = passport.Objects.FirstOrDefault(x => x.WorkObject.Id == workObject.Id);
                if (objectsInPassport != null)
                {
                    record.AwpReadinessPassport = passport;
                    record.PassportDate = passport.ChangeStatusDate;
                }
            }
            if (readinessAct != null)
            {
                var objectsInAct = readinessAct.Objects.FirstOrDefault(x => x.WorkObject.Id == workObject.Id);
                if (objectsInAct != null)
                {
                    record.ReadinessAct = readinessAct;
                    record.ReadinessActDate = readinessAct.ChangeStatusDate;
                }
            }

            var evaluationSheets = await repository.Find<EvaluationSheet>(x => x.Awp.Id == awpId && x.ActStatus.Id == IActStatus.Keys.Signed
                && x.WorkObject.Id == workObject.Id);
            if (evaluationSheets.Count > 0)
            {
                var evaluationSheet = evaluationSheets.OrderByDescending(x => x.ChangeStatusDate).First();
                record.EvaluationSheet = evaluationSheet;
                record.EvaluationSheetDate = evaluationSheet.ChangeStatusDate;
                record.IndexReadiness = evaluationSheet.ObjectReadinessIndex;
                record.HasEvaluationSheetProblem = evaluationSheet.Requirements.Any(x => !string.IsNullOrEmpty(x.ProblemComment));
            }
        }
        repository.Create(record);
    }
    
}
