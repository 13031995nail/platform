namespace Tmobis.Business.Core.Models.Changelogs;
using Tmobis.Core.Models;

public interface IChangelog : IRegisteredEntity
{
    public string Added { get; set; }

    public string Changes { get; set; }

    public string Fixes { get; set; }
      
    public string Hotfix { get; set; }
      
    public DateTime? UpdateDate { get; set; }
      
    public DateTime? VersionDate { get; set; }
      
    public string VersionName { get; set; }
      
    public string VersionNumber { get; set; }
}

