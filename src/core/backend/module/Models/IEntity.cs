namespace Tmobis.Core.Models;

using System.ComponentModel.DataAnnotations;

public interface IEntity : IAuditable
{
    [Key]
    Guid Id { get; set; }
}
