namespace Tmobis.Business.Process.Documents;

using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Process.Documents.Base;

public class OrgActNumberBuilder : ActNumberBuilder
{
    public override string GetActNumber(IBaseActBase baseDoc)
    {
        if (baseDoc is IBaseAct document)
        {
            var municipalityAcronym = document.Organization is IOrganization organization
                ? organization.Hierarchy?.Acronym ?? organization.Acronym
                : document.Organization?.Acronym;

            return $"{document.CreateTime?.Year}-{municipalityAcronym}-{document?.ActType?.Acronym}-{document?.ActNumberIndex}";
        }

        return base.GetActNumber(baseDoc);
    }
}