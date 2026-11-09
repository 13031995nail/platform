namespace Tmobis.Business.Core.Models.Files;
using Tmobis.Core.Entities;
using Tmobis.Core.Models;

[InternalEntity]
public interface IFile : IRegisteredEntity
{
  public string Name { get; set; }

  public ulong Size { get; set; }

  public bool IsDeleteProtected { get; set; }
}
