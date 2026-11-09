namespace Tmobis.Business.Process.Documents;

using Tmobis.Business.Process.Documents.Base;

public class ActNumberBuilder : IActNumberBuilder
{
    public virtual string GetActNumber(IBaseActBase document)
    => $"{document.CreateTime?.Year}-{document?.Assignee?.Town?.CityAcronym}-{document?.ActType?.Acronym}-{document?.ActNumberIndex}";
}