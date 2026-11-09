namespace Tmobis.Integrations.Messengers.Models;

using System.ComponentModel.DataAnnotations;
using Tmobis.Core.Models;

public class Messenger : Dictionary, IMessenger
{
    [StringLength(50)]
    public override string? Code { get; set; }

    [StringLength(100)]
    public override string Name { get; set; } = string.Empty;
}
