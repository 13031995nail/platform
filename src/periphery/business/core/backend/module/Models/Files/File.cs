namespace Tmobis.Business.Core.Models.Files;

using Tmobis.Core.Models;

public class File
: BusinessEntity, IFile
{
  public virtual string Name { get; set; } = string.Empty;

  public virtual ulong Size { get; set; }

  public virtual bool IsDeleteProtected { get; set; }
}
