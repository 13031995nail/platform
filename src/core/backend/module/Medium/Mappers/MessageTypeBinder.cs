namespace Tmobis.Core.Medium.Mappers;

public class MessageTypeBinder(IMessageRegister messageRegister) : IMessageTypeBinder
{
    private readonly IMessageRegister MessageRegister = messageRegister;

    public Type BindToType(string? assemblyName, string typeName) => MessageRegister.MessageTypes[typeName];

    public void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
    {
        assemblyName = null;
        typeName = serializedType.FullName;
    }
}
