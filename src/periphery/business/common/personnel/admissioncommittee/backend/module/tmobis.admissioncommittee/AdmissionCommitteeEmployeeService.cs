namespace Tmobis.Business.Common.Personnel;

using Tmobis.Core.Repositories;

public class AdmissionCommitteeEmployeeService(IRepository repository)
{

    public async Task UpdatePlaces(AdmissionCommitteeEmployee entity)
    {
        var places = await repository.Find<PlaceOfEmployment>(
            x => x.Employee.Id == entity.Employee.Id &&
                x.Organization.Hierarchy.Id == entity.AdmissionCommittee.Organization.Id);
        if (places.Count > 0)
        {
            entity.PlacesOfWork = string.Join(",", places.Select(x => x.Organization.Name));
        }
    }
}
