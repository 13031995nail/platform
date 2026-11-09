namespace Tmobis.Business.Core.Models.Employees;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tmobis.Core.Entities;
using Towns;
using Users;
using Tmobis.Core.Models;

public interface IEmployee : IRegisteredEntity, IActualEntity, IWTown
{
    [StringLength(150)]
    string? Email { get; set; }

    [NotMapped]
    [ComputedProperty("Tmobis.Business.Core.Models.Employees.EmployeeLogic.Fio")]
    string Fio { get; }

    [StringLength(50)]
    string? FirstName { get; set; }

    [StringLength(50)]
    string? LastName { get; set; }

    [StringLength(50)]
    string? Patronymic { get; set; }

    [StringLength(50)]
    string? Phone { get; set; }

    string? Photos { get; set; }

    Guid? UserId { get; set; }

    IUser? User { get; set; }

    public class Views
    {
        public static readonly EntityView EmployeeE
            = new(
                nameof(EmployeeE),
                [nameof(Town)],
                searchProperties: [
                    nameof(LastName),
                    nameof(FirstName),
                    nameof(Email),
                ]);
    }
}

public static class EmployeeLogic
{
    public static string Fio(IEmployee employee) => $"{employee.LastName} {(string.IsNullOrWhiteSpace(employee.FirstName)
        ? string.Empty
        : char.ToUpper(employee.FirstName[0])
          + "." + (string.IsNullOrWhiteSpace(employee.Patronymic)
              ? string.Empty
              : char.ToUpper(employee.Patronymic[0]) + "."))}".Trim();
}