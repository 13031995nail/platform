namespace Tmobis.Core.Medium.Mappers;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Modules;

public class MessageRegister : IMessageRegister
{
    public ReadOnlyDictionary<string, Type> MessageTypes { get; set; }

    public MessageRegister(IModuleRegister moduleRegisters)
    {
        Dictionary<string, Type> messageTypesDictionary = [];
        foreach (var module in moduleRegisters)
        {
            if (module is not IMessageModule messageModule) continue;
            var messageTypes = messageModule
                .GetType()
                .Assembly
                .GetTypes()
                .Where(type =>
                    type.IsAssignableTo(typeof(IMessage))
                    && !type.IsAbstract
                    && type.FullName != null);
            foreach (var type in messageTypes)
            {
                messageTypesDictionary.Add(type.FullName!, type);
            }
        }
        MessageTypes = messageTypesDictionary.AsReadOnly();
    }

    public Type? GetTypeByName(string name)
    {
        MessageTypes.TryGetValue(name, out Type? type);
        return type;
    }
}
