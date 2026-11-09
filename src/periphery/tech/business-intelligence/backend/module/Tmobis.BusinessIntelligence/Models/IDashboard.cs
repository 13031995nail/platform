namespace Tmobis.BusinessIntelligence;
using Tmobis.Core.Models;

public interface IDashboard
: IEntity
{
    public string? TmobisDashboardId { get; set; }

    public string? MetabaseDashboardId  { get; set; }

    public string? Note { get; set; }
}
