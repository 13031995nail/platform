namespace Tmobis.Frameworks.Flexberry.Converters.OldFile.Converters;
using System.Text.RegularExpressions;
using File = Tmobis.Business.Core.Flexberry.Models.File;

public partial class OldWebFileConverter
: IOldWebFileConverter
{
    public virtual void ConvertOldWebFile(File file)
    {
        ArgumentNullException.ThrowIfNull(file);
        file.ExternalId = GetId(file);
        file.Name = file?.Value?.Name;
        file.Size = (ulong)(file?.Value?.Size ?? 0);
    }

    protected virtual Guid GetId(File file)
    {
        var guid = Guid.Empty;
        var url = file?.Value?.Url;

        if (string.IsNullOrEmpty(url))
        {
            return guid;
        }

        var regex = RegexId();
        var match = regex.Match(url);
        if (!match.Success)
        {
            return guid;
        }

        var guidStr = match.Groups[1]?.Value?.Replace("}", "")?.Replace("{", "");
        if (!Guid.TryParse(guidStr, out guid))
        {
            guid = Guid.Empty;
        }

        return guid;
    }

    [GeneratedRegex(@"fileUploadKey=(.+)&file[nN]ame")]
    private static partial Regex RegexId();
}
