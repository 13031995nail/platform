namespace Tmobis.Business.Common.Resources;
using Tmobis.Core.Models;
using Tmobis.Business.Common.Contacts;

public class VerificationRequierement : Dictionary, IVerificationRequierement
{
    public virtual bool IsCalculated { get; set; } = false;

    public virtual OrganizationType OrganizationType { get; set; }

    public virtual string ShortReqName { get; set; }

    public virtual string ReqNumber { get; set; }

    public virtual string ShortIndicatorName { get; set; }

    public virtual string IndicatorName { get; set; }

    public virtual string SupportingDocName { get; set; }

    public virtual System.Decimal? Weight { get; set; }
}
