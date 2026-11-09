namespace Tmobis.Core.Medium.Producers;

using System.Collections.ObjectModel;
using System.Collections.Generic;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Modules;

public class ProducerTypeRegistry : IProducerTypeRegistry
{
    public ReadOnlyDictionary<string, Type> ProducerTypes { get; set; }

    public ProducerTypeRegistry(IModuleRegister moduleRegisters)
    {
        Dictionary<string, Type> messageTypesDictionary = [];
        foreach (var module in moduleRegisters)
        {
            if (module is IProducerModule messageModule)
            {
                var messageTypes = messageModule
                  .GetType()
                  .Assembly
                  .GetTypes()
                  .Where(type =>
                    type.IsAssignableTo(typeof(IMessageProducer))
                    && type.FullName != null);
                foreach (var type in messageTypes)
                {
                    messageTypesDictionary.Add(type.Name, type);
                }
            }
        }
        ProducerTypes = messageTypesDictionary.AsReadOnly();
    }

    public Type? GetTypeByName(string name)
    {
        ProducerTypes.TryGetValue(name, out Type? type);
        return type;
    }
}
