namespace Tmobis.Core.Medium.Mappers;

public interface IMessageProducerRegister
{
    Type? GetProducerByMessageType(Type messageType);
}
