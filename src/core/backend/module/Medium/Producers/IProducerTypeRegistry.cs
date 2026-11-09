namespace Tmobis.Core.Medium.Producers;

using System.Collections.ObjectModel;

public interface IProducerTypeRegistry
{
    public ReadOnlyDictionary<string, Type> ProducerTypes { get; set; }

    public Type? GetTypeByName(string name);
}
