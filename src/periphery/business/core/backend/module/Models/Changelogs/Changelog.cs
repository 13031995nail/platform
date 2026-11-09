namespace Tmobis.Business.Core.Models.Changelogs;
using Tmobis.Core.Models;

public class Changelog : BusinessEntity, IChangelog
{
    /// <inheritdoc/>
    public virtual string Added { get; set; }

    /// <inheritdoc/>
    public virtual string Changes { get; set; }

    /// <inheritdoc/>
    public virtual string Fixes { get; set; }

    /// <inheritdoc/>
    public virtual string Hotfix { get; set; }

    /// <inheritdoc/>
    public virtual DateTime? UpdateDate { get; set; }

    /// <inheritdoc/>
    public virtual DateTime? VersionDate { get; set; }

    /// <inheritdoc/>
    public virtual string VersionName { get; set; }

    /// <inheritdoc/>
    public virtual string VersionNumber { get; set; }
}