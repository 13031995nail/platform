namespace Tmobis.Core.Medium.Mappers;

public interface IMessageListRegister
{
    IReadOnlyList<Type> GetListenedMessageList(string listenerName);
}
