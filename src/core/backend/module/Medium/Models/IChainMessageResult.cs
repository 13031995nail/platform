namespace Tmobis.Core.Medium.Models;

public interface IChainMessageResult : IMessageResult
{
    bool Continue { get; init; }
}