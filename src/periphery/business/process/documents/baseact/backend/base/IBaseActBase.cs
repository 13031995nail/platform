namespace Tmobis.Business.Process.Documents.Base;
using Tmobis.Business.Core.Models.Employees;
using Tmobis.Core.Models;

public interface IBaseActBase : IEntity
{
  DateTime RegDate { get; set; }

  IActStatus Status { get; set; }

  IActType ActType { get; set; }

  string ActNumber { get; set; }

  int? ActNumberIndex { get; set; }

  DateTime? ChangeStatusDate { get; set; }

  IEmployee? Assignee { get; set; }
}