namespace Tmobis.Business.Core.Models.Employees;

using Tmobis.Business.Core.Models.Towns;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Core.Models;

public class Employee
: ActualEntity, IEmployee
{
    /// <inheritdoc/>
    public virtual string? Email { get; set; }
    
    /// <inheritdoc/>
    public virtual string Fio => string.Format(
    "{0} {1}",
    LastName,
    string.IsNullOrWhiteSpace(FirstName)
        ? string.Empty
        : char.ToUpper(FirstName[0]) 
    + "." + (string.IsNullOrWhiteSpace(Patronymic) 
        ? string.Empty 
        : char.ToUpper(Patronymic[0]) + ".")).Trim();

    /// <inheritdoc/>
    public virtual string? FirstName { get; set; }

    /// <inheritdoc/>
    public virtual string? LastName { get; set; }

    /// <inheritdoc/>
    public virtual string? Patronymic { get; set; }

    /// <inheritdoc/>
    public virtual string? Phone { get; set; }

    /// <inheritdoc/>
    public virtual string? Photos { get; set; }

    public virtual Guid? UserId { get; set; }

    /// <inheritdoc/>
    public virtual IUser? User { get; set; }

    public virtual Guid TownId { get; set; }

    /// <inheritdoc/>
    public virtual ITown Town { get; set; }
}