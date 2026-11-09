namespace Tmobis.Core.Converters;

public interface IPropertyNameConverter
{
    string GetBackendPropertyName(string? propertyName);

    string GetFrontendPropertyName(string? propertyName);
}
