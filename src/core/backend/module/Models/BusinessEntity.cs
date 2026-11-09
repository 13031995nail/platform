namespace Tmobis.Core.Models;

using System.ComponentModel.DataAnnotations;

public abstract class BusinessEntity
: IBusinessEntity
{
    /// <inheritdoc/>
    [Key]
    public virtual Guid Id { get; set; }

    /// <inheritdoc/>
    public virtual DateTime? CreateTime { get; set; }

    /// <inheritdoc/>
    [StringLength(255)]
    public virtual string? Creator { get; set; } = string.Empty;

    /// <inheritdoc/>
    public virtual DateTime? EditTime { get; set; }

    /// <inheritdoc/>
    [StringLength(255)]
    public virtual string? Editor { get; set; }
}
