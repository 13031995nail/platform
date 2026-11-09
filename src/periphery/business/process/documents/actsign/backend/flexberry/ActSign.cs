namespace Tmobis.Business.Process.Documents.Flexberry;

using ICSSoft.STORMNET;
using System.Runtime.Serialization;
using Tmobis.Business.Core.Flexberry.Models;
using Tmobis.Business.Process.Documents.Base;
using Tmobis.Frameworks.Flexberry.Entities;

/// <summary>
/// Подпись документа.
/// </summary>
[ClassStorage("ActSign")]
[AutoAltered]
[Caption("Подпись документа")]
[AccessType(AccessType.none)]
[View("ActSignE", [
  "Signature", "SignDate", "SignRole", "SignType", "Employee",
  "Fio", "Address", "Email",
  "Phone", "Passport", "PositionName", "PartnerName"
])]
public abstract class ActSign : FlexberryEntity, IActSign
{
    [StrLen(50)]
    public virtual string Signature { get; set; }

    public virtual DateTime? SignDate { get; set; }

    [NotNull]
    [PropertyStorage(["SignRole"])]
    public virtual SignRole? SignRole { get; set; }

    [PropertyStorage(["SignType"])]
    public virtual SignType? SignType { get; set; }

    [PropertyStorage(["Employee"])]

    public virtual Employee? Employee { get; set; }

    public virtual string Fio { get; set; }

    public virtual string Address { get; set; }

    public virtual string Email { get; set; }

    public virtual string Phone { get; set; }

    public virtual string Passport { get; set; }

    public virtual string PositionName { get; set; }

    public virtual string PartnerName { get; set; }

    [NotStored]
    [StrLen(150)]
    public virtual string SignDateString
    {
        get => this.SignDate?.ToString("dd.MM.yyyy");

        set
        {
        }
    }

    [NotStored]
    [IgnoreDataMember]
    ISignRole? IActSign.SignRole
    {
        get => SignRole;
        set
        {
            if (value is SignRole role)
            {
                SignRole = role;
            }
        }
    }

    [NotStored]
    [IgnoreDataMember]
    ISignType? IActSign.SignType
    {
        get => SignType;
        set
        {
            if (value is SignType type)
            {
                SignType = type;
            }
        }
    }

    [NotStored]
    [IgnoreDataMember]
    Core.Models.Employees.IEmployee? IActSign.Employee
    {
        get => Employee;
        set
        {
            if (value is Employee employee)
            {
                Employee = employee;
            }
        }
    }


    /// <summary>
    /// Class views container.
    /// </summary>
    public new class Views
    {
        /// <summary>
        /// "ActSignE" view.
        /// </summary>
        public static View ActSignE
        => Information.GetView("ActSignE", typeof(ActSign));
    }
}
