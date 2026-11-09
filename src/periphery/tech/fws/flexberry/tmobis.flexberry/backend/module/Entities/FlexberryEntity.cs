namespace Tmobis.Frameworks.Flexberry.Entities;
using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business.Audit;
using ICSSoft.STORMNET.KeyGen;
using Tmobis.Core.Models;

[NotStored]
[Serializable]
public abstract class FlexberryEntity
: DataObject, IStoredEntity, IDataObjectWithAuditFields
{
  /// <inheritdoc/>
  [NotStored]
  [DataServiceExpression("ICSSoft.STORMNET.Business.MSSQLDataService, ICSSoft.STORMNET.Business.MSSQLDataService", "StormMainObjectKey")]
  [DataServiceExpression("ICSSoft.STORMNET.Business.PostgresDataService, ICSSoft.STORMNET.Business.PostgresDataService", "StormMainObjectKey")]
  public Guid Id
  {
    get => ((KeyGuid)__PrimaryKey).Guid;
    set
    {
      if (((KeyGuid)__PrimaryKey).Guid != value)
      {
        __PrimaryKey = new KeyGuid(value);
      }
    }
  }

  /// <inheritdoc/>
  public virtual DateTime? CreateTime { get; set; }

  /// <inheritdoc/>
  [StrLen(255)]
  public virtual string? Creator { get; set; } = string.Empty;

  /// <inheritdoc/>
  public virtual DateTime? EditTime { get; set; }

  /// <inheritdoc/>
  [StrLen(255)]
  public virtual string? Editor { get; set; }
}
