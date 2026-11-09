namespace Tmobis.Integrations.Messengers.Models;

using System.ComponentModel.DataAnnotations;
using Tmobis.Business.Core.Models.Employees;
using Tmobis.Core.Models;

public class EmployeeMessenger : BusinessEntity, IEmployeeMessenger
{
    public virtual Guid EmployeeId { get; set; }

    public virtual required IEmployee Employee { get; set; }

    public virtual Guid MessengerId { get; set; }

    public virtual required IMessenger Messenger { get; set; }

    [StringLength(255)]
    public virtual string ExternalId { get; set; } = string.Empty;

    public virtual bool IsPreferred { get; set; }
}
