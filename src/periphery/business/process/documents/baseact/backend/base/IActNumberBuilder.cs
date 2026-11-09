namespace Tmobis.Business.Process.Documents;

using Tmobis.Business.Process.Documents.Base;

public interface IActNumberBuilder
{
    string GetActNumber(IBaseActBase document);
}