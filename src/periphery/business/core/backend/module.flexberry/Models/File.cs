namespace Tmobis.Business.Core.Flexberry.Models;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using ICSSoft.STORMNET.UserDataTypes;
using Tmobis.Frameworks.Flexberry.Entities;
using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Business.Core.Models.Files;

[AutoAltered]
[AccessType(AccessType.@this)]
[View("FileD", ["ExternalId", "Name", "Size", "IsDeleteProtected"])]
[View("FileOld", ["*"])]
[View("FileInfo", ["Name", "Creator", "CreateTime"])]
[BusinessServer(typeof(FileBS), DataServiceObjectEvents.OnAllEvents)]
public class File : FlexberryEntity, IFile
{
  private Guid? fExternalId;
  [Obsolete]
  public virtual WebFile? Value { get; set; }

  public virtual Guid? ExternalId 
  {
    get => fExternalId ?? Id; 
    set => fExternalId = value;
  }

  public virtual string Name { get; set; } = string.Empty;

  public virtual ulong Size { get; set; }

  public virtual bool IsDeleteProtected { get; set; }

  /// <summary>
  /// Class views container.
  /// </summary>
  public class Views
  {
      public static readonly Tmobis.Core.Entities.EntityView FileInfoEntityView = new(nameof(FileInfo));
    public static View FileInfo => Information.GetView(nameof(FileInfo), typeof(File));

    /// <summary>
    /// "FileD" view.
    /// </summary>
    public static View FileD => Information.GetView("FileD", typeof(File));

    /// <summary>
    /// "FileOld" view.
    /// </summary>
    public static View FileOld => Information.GetView("FileOld", typeof(File));
  }
}
