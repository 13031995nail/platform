namespace Tmobis.Business.Process.Documents.Base;
using Tmobis.Business.Core.Models.Employees;
using Tmobis.Core.Models;

public interface IActSign : IEntity
{
  string Signature { get; set; }

  DateTime? SignDate { get; set; }

  ISignType? SignType { get; set; }

  IEmployee? Employee { get; set; }

  ISignRole? SignRole { get; set; }

  string Fio { get; set; }

  string Address { get; set; }

  string Email { get; set; }

  string Phone { get; set; }

  string Passport { get; set; }

  string PositionName { get; set; }

  string PartnerName { get; set; }
}
