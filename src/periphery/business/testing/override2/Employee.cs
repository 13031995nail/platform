namespace Tmobis.Business.Override2;

using Core.Models.Employees;

public interface IEmployeeWBirthDate
: IEmployee
{
    DateTime BirthDate { get; set; }
}