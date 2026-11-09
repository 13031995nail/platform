namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Process.Documents.Flexberry;

[ClassStorage("RedinessAct")]
[View("RedinessActBaseE", [
  "ActStatus", "Organization", "Organization.Hierarchy",
  "Organization.OrganizationType", "Awp"
])]
public class RedinessActBase : BaseAct, IWAwp
{
    [NotNull]
    [PropertyStorage(["Awp"])]
    public virtual Awp Awp { get; set; }
}

