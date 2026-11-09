namespace Tmobis.Core.Medium.Mappers;
using System.Collections.ObjectModel;

public interface IMessageRegister
{
    public ReadOnlyDictionary<string, Type> MessageTypes { get; set; }

    public Type? GetTypeByName(string name);
}
