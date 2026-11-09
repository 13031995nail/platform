namespace Tmobis.Core.Medium.Mappers;

using Microsoft.Extensions.Options;
using Tmobis.Core.Entities;
using Tmobis.Core.Medium.Options;
using Tmobis.Core.Medium.Producers;

public class MessageProducerRegister : MessageBaseRegister<MessageProducerMapping, MessageProducerOptions>, IMessageProducerRegister
{
    private Dictionary<Type, Type?> Mappers { get; set; }
    public MessageProducerRegister(
        IEnumerable<MessageProducerMapping> mappings,
        IOptions<MessageProducerOptions> options,
        IMessageRegister messageRegister,
        IProducerTypeRegistry typeProducer) : base()
    {
        var producerMappings = mappings.ToDictionary(g => g.MessageType, g => typeProducer.GetTypeByName(g.Name));
        var overrideOption = options.Value;
        if (overrideOption != null && overrideOption.Overrides != null && overrideOption.Overrides.Count > 0)
        {
            foreach (var (messageTypeName, producerName) in overrideOption.Overrides)
            {
                if (!string.IsNullOrEmpty(messageTypeName) && producerName != null)
                {
                    Type? messageType = messageRegister.GetTypeByName(messageTypeName);
                    if (messageType != null)
                        producerMappings[messageType] = typeProducer.GetTypeByName(producerName);
                }
            }
        }
        Mappers = producerMappings;       
    }

    public Type? GetProducerByMessageType(Type messageType)
    {
        if (Mappers.TryGetValue(messageType, out Type? producerType))
            return producerType;
        return null;
    }
}
