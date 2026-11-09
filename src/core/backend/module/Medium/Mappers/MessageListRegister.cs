namespace Tmobis.Core.Medium.Mappers;

using Microsoft.Extensions.Options;
using Tmobis.Core.Medium.Options;

public class MessageListRegister : MessageBaseRegister<MessageListenerMapping, MessageListRegisterOptions>, IMessageListRegister
{
    private readonly Dictionary<string, List<Type>> reverseMapping = new();
    public MessageListRegister(
        IEnumerable<MessageListenerMapping> mappings,
        IOptions<MessageListRegisterOptions> options,
        IMessageRegister messageRegister) : base()
    {
        var listenerMappings = mappings.GroupBy(m => m.MessageType).ToDictionary(g => g.Key, g => g.Select(m => m.Name).ToList());

        var overrideOption = options.Value;
        if (overrideOption != null && overrideOption.Overrides != null && overrideOption.Overrides.Count > 0)
        {
            foreach (var (messageTypeName, listenerNames) in overrideOption.Overrides)
            {
                if (!string.IsNullOrEmpty(messageTypeName) && listenerNames != null && listenerNames.Count > 0)
                {
                    Type? messageType = messageRegister.GetTypeByName(messageTypeName);
                    if (messageType != null)
                        listenerMappings[messageType] = listenerNames.ToList();
                }
            }
        }

        foreach (var mapping in listenerMappings)
        {
            var messageType = mapping.Key;
            foreach (var listenerName in mapping.Value)
            {
                if (!reverseMapping.TryGetValue(listenerName, out var types))
                {
                    types = new List<Type>();
                    reverseMapping[listenerName] = types;
                }
                types.Add(messageType);
            }
        }
    }

    public IReadOnlyList<Type> GetListenedMessageList(string listenerName)
    {
        ArgumentException.ThrowIfNullOrEmpty(listenerName);
        if (reverseMapping.TryGetValue(listenerName, out var types))
        {
            return types.ToList();
        }
        return new List<Type>();
    }
}
