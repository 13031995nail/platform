namespace Tmobis.Integrations.Messengers.Models;

using System.ComponentModel.DataAnnotations;
using Tmobis.Business.Core.Models.Employees;
using Tmobis.Core.Models;

public interface IEmployeeMessenger : IRegisteredEntity
{
    Guid EmployeeId { get; set; }

    IEmployee Employee { get; set; }

    Guid MessengerId { get; set; }

    IMessenger Messenger { get; set; }

    [StringLength(255)]
    string ExternalId { get; set; }

    bool IsPreferred { get; set; }
}
