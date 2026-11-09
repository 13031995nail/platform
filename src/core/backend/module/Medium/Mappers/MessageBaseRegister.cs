namespace Tmobis.Core.Medium.Mappers;

using Microsoft.Extensions.Options;
using Tmobis.Core.Medium.Options;

public class MessageBaseRegister<T1, T2> () 
    where T1 : IMessageMapping
    where T2 : MessageOptions
{
}
