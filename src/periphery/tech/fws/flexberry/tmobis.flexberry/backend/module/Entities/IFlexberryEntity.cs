namespace Tmobis.Frameworks.Flexberry.Entities;
using Tmobis.Core.Models;

public interface IFlexberryEntity : IStoredEntity
{
    object __PrimaryKey { get; set; }
}
