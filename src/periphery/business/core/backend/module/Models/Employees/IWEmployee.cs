namespace Tmobis.Business.Core.Models.Employees;

public interface IWEmployee
{
  Guid EmployeeId { get; set; }

  IEmployee Employee { get; set; }
}
