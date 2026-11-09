namespace Tmobis.Business.Process.Documents.Flexberry;

using Tmobis.Business.Core.Models.Files;
using Tmobis.Core.Repositories;

public class OnFileDeletedConsumer(IRepository repository)
    : Core.Flexberry.Consumers.Files.OnFileDeletedConsumer(repository)
{
    protected override bool IsProtected(IFile file)
    => base.IsProtected(file) || file?.Name == "signature.png";
}
