namespace Tmobis.Core.Converters;

using System.Text.RegularExpressions;

public partial class PropertyNameConverter
: IPropertyNameConverter
{
    public string GetBackendPropertyName(string? propertyName)
    => string.Concat(
        propertyName?[..1].ToUpper(),
        BackendPropRegex().Replace(
            propertyName?[1..] ?? "", (f) => f.Value.ToUpper()));

    public string GetFrontendPropertyName(string? propertyName)
    => string.Concat(
                propertyName?[..1].ToLower(),
                propertyName?[1..]);

    [GeneratedRegex("\\.\\w")]
    private static partial Regex BackendPropRegex();
}
