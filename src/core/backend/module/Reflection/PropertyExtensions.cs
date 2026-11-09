namespace Tmobis.Core.Models;

using System.Reflection;

public static class PropertyExtensions
{
    extension(Type type)
    {
        public IEnumerable<PropertyInfo> GetPublicProperties()
        {
            if (!type.IsInterface)
                return type.GetProperties();

            return new[] { type }
                .Concat(type.GetInterfaces())
                .SelectMany(i => i.GetProperties());
        }

        public PropertyInfo? GetInheritedProperty(string propertyName)
        => type.GetPublicProperties().FirstOrDefault(p => p.Name == propertyName);
    }
}
