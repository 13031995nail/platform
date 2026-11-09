namespace Tmobis.Core.Models;

using System.ComponentModel.DataAnnotations;

public interface IAuditable
{
    [StringLength(255)]
    DateTime? CreateTime { get; set; }

    string? Creator { get; set; }

    [StringLength(255)]
    DateTime? EditTime { get; set; }

    string? Editor { get; set; }
}
