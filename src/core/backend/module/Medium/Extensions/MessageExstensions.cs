namespace Tmobis.Core.Medium.Extensions;

using Tmobis.Core.Medium.Models;

public static class MessageExtensions
{
    public static Type? GetMessageResultType(this IMessage message)
    => GetMessageResultType(message.GetType());

    public static Type? GetMessageResultType(Type messageType)
    => messageType.GetInterfaces()
        .FirstOrDefault(i =>
            i.IsGenericType
            && i.IsAssignableTo(typeof(IMessage))
            && i.GenericTypeArguments[0]
                .IsAssignableTo(typeof(IMessageResult)))
                ?.GenericTypeArguments[0];

    public static void CopyContext(this IMessage to, IMessage from)
    {
        foreach (var key in from.Context.Keys.Where(key => !to.Context.ContainsKey(key)))
        {
            to.Context.Add(key, from.Context[key]);
        }
    }
}