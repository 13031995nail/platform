namespace Tmobis.Reports.Models;

using System.ComponentModel.DataAnnotations;
using Tmobis.Core.Models;

public interface IReport : IRegisteredEntity
{
    [StringLength(255)] public string Name { get; set; }

    public string? Description { get; set; }

    [StringLength(255)] public string? VirtualName { get; set; }

    public string? UserScript { get; set; }
}
