namespace Tmobis.Business.Override1;

using Tmobis.Core.Models;

public interface IEmployeeType
    : IDictionary, IRegisteredEntity;

public class EmployeeType
    : Dictionary, IEmployeeType;

public interface IWEmployeeType
{
    Guid EmployeeTypeId { get; set; }
    IEmployeeType EmployeeType { get; set; }
}