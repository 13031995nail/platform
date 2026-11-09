namespace Tmobis.Business.Process.Documents;

using Common.Personnel;
using Tmobis.Business.Core.Flexberry.Models;
using Core.Models.Users;
using Core.Providers.Users;
using Flexberry;
using Tmobis.Core.Entities.Consumers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

public class OnAwpReadinessPassportCreated(
    IRepository repository,
    IUserProvider userProvider)
: OnEntityUpdated<EntityCreated<AwpReadinessPassport>, AwpReadinessPassport>(null)
{
    protected override async ValueTask<List<IEntity>> ChangeEntity(AwpReadinessPassport entity)
    {
        var toUpdate = new List<IEntity>();
        toUpdate.AddRange(await CreateObjects(entity));
        toUpdate.AddRange(await CreateSigns(entity));
        return toUpdate;
    }

    protected virtual async Task<List<AwpReadinessPassportObject>>
  CreateObjects(AwpReadinessPassport entity)
    {
        var objects = new List<AwpReadinessPassportObject>();
        var readinessActId = entity?.ReadinessAct?.Id;

        if (!readinessActId.HasValue)
            return [];

        var readinessAct = await repository.GetById<RedinessAct>(readinessActId.Value);
        if (readinessAct == null)
            return [];

        int idx = 1;
        foreach (var actObject in readinessAct.Objects)
        {
            var passportObject = repository.MakeEntity<AwpReadinessPassportObject>();
            passportObject.WorkObject = actObject.WorkObject;
            passportObject.Number = idx;
            passportObject.Act = entity;
            objects.Add(passportObject);
            repository.Create(passportObject);
            idx++;
        }
        entity.PartnerReadiness = readinessAct.PartnerReadiness;
        return objects;
    }

    /// <summary>
    /// При создании паспорта автоматически в подписанты добавляется создавший сотрудник
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected virtual async Task<List<AwpReadinessPassportSign>> CreateSigns(AwpReadinessPassport entity)
    {
        var signs = new List<AwpReadinessPassportSign>();
        if (!string.IsNullOrEmpty(entity.Creator))
        {
            IUser? user = await userProvider.GetUserByLogin(entity.Creator);
            if (user != null)
            {
                var existingEmployee = await repository.FirstOrDefault<Employee>(x => x.User != null && x.User.Id == user.Id);
                if (existingEmployee != null)
                {
                    signs.Add(await CreateSign(entity, existingEmployee, ISignRole.Keys.CommissionCreator));
                }
            }
        }

        return signs;
    }

    private async Task<AwpReadinessPassportSign> CreateSign(AwpReadinessPassport entity, Employee employee, Guid role)
    {
        var sign = repository.MakeEntity<AwpReadinessPassportSign>();
        sign.Act = entity;
        sign.Employee = employee;
        sign.SignRole = await repository.GetById<SignRole>(role);
        sign.Fio = employee.Fio;
        repository.Create(sign);
        // сначала ищем места работы по организации
        var poe = await repository.FirstOrDefault<PlaceOfEmployment>(x => x.Employee.Id == employee.Id && x.Organization.Id == entity.Organization.Id);
        if (poe == null)
        {
            // затем ищем места работы по иерархии
            var MO = entity.Organization.Hierarchy == null ? entity.Organization.Id : entity.Organization.Hierarchy.Id;
            poe = await repository.FirstOrDefault<PlaceOfEmployment>(x => x.Employee.Id == employee.Id
                && (x.Organization.Id == MO || x.Organization.Hierarchy != null && x.Organization.Hierarchy.Id == MO));
        }
        if (poe != null)
        {
            sign.PositionName = poe.Position.Name;
            sign.PartnerName = poe.Organization.Name;
        }
        return sign;
    }
}
