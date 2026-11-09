namespace Tmobis.Offline.Backups.Helpers
{
  using ICSSoft.STORMNET;
  using ICSSoft.STORMNET.KeyGen;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;

  public static class DOInfoHelper
  {
    private const string RelationsAttributeName = "CalcPropertyRelationsAttribute";
    private const string DetailsAttributeName = "CalcPropertyDetailsAttribute";

    public static bool DOEqual(DataObject do1, DataObject do2)
    {
      return do1 == null
          ? do2 == null
          : do2 == null
              ? false
              : (do1.__PrimaryKey as KeyGuid).Guid == (do2.__PrimaryKey as KeyGuid).Guid;
    }

    public static string GetServerPropertyName(string clientPropName)
    {
      if (string.IsNullOrWhiteSpace(clientPropName))
      {
        return clientPropName;
      }

      var result = clientPropName.Trim();

      return result[0].ToString().ToUpper() + result.Substring(1);
    }

    public static string GetModelNameByType(Type classType)
    {
      return classType.FullName.Replace(".", string.Empty);
    }

    public static Type CheckPropertyPathExists(Type classType, string propertyPath)
    {
      if (string.IsNullOrWhiteSpace(propertyPath) || classType == null)
      {
        return null;
      }

      var propNames = propertyPath.Split('.');
      var curPropType = classType;
      for (var i = 0; i < propNames.Length; i++)
      {
        var curProp = curPropType.GetProperty(propNames[i]);
        if (curProp == null)
        {
          return null;
        }

        curPropType = curProp.PropertyType;
      }

      return curPropType;
    }

    public static List<string> GetRelationsPropertiesForCalculatedFields(Type classType, string[] notStoredProps = null)
    {
      if (classType == null)
      {
        return new List<string>();
      }

      var propsInfo = (notStoredProps ?? Information.GetNotStorablePropertyNames(classType)).Select(x => classType.GetProperty(x)).Where(x => x != null).ToList();
      var props = propsInfo.SelectMany(propertyInfo => GetAttributesRelationsPaths(propertyInfo)).Distinct().ToList();
      return props;
    }

    public static List<string> GetDetailPropertiesForCalculatedFields(string detailName, Type classType, string[] notStoredProps = null)
    {
      if (classType == null)
      {
        return new List<string>();
      }

      var propsInfo = (notStoredProps ?? Information.GetNotStorablePropertyNames(classType)).Select(x => classType.GetProperty(x)).Where(x => x != null).ToList();
      var props = propsInfo.SelectMany(propertyInfo => GetAttributesDetailsPaths(propertyInfo, detailName)).Distinct().ToList();
      return props;
    }

    private static IEnumerable<string> GetAttributesRelationsPaths(PropertyInfo propertyInfo)
    {
      foreach (var attribute in propertyInfo.GetCustomAttributes(true))
      {
        if (!string.Equals(attribute.GetType().Name, RelationsAttributeName, StringComparison.Ordinal))
        {
          continue;
        }

        var paths = TryGetStringArray(attribute, "RelationsPropertiesPaths");
        if (paths == null)
        {
          continue;
        }

        foreach (var path in paths.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
          yield return path;
        }
      }
    }

    private static IEnumerable<string> GetAttributesDetailsPaths(PropertyInfo propertyInfo, string detailName)
    {
      foreach (var attribute in propertyInfo.GetCustomAttributes(true))
      {
        if (!string.Equals(attribute.GetType().Name, DetailsAttributeName, StringComparison.Ordinal))
        {
          continue;
        }

        var attributeDetailName = TryGetString(attribute, "DetailName");
        if (!string.Equals(attributeDetailName, detailName, StringComparison.Ordinal))
        {
          continue;
        }

        var paths = TryGetStringArray(attribute, "DetailPropertiesPaths");
        if (paths == null)
        {
          continue;
        }

        foreach (var path in paths.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
          yield return path;
        }
      }
    }

    private static string TryGetString(object attribute, string memberName)
    {
      if (attribute == null)
      {
        return null;
      }

      var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
      var property = attribute.GetType().GetProperty(memberName, flags);
      if (property != null && property.PropertyType == typeof(string))
      {
        return property.GetValue(attribute) as string;
      }

      var field = attribute.GetType().GetField(memberName, flags);
      if (field != null && field.FieldType == typeof(string))
      {
        return field.GetValue(attribute) as string;
      }

      return null;
    }

    private static string[] TryGetStringArray(object attribute, string memberName)
    {
      if (attribute == null)
      {
        return null;
      }

      var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
      var property = attribute.GetType().GetProperty(memberName, flags);
      if (property != null)
      {
        if (property.PropertyType == typeof(string[]))
        {
          return property.GetValue(attribute) as string[];
        }

        if (typeof(IEnumerable<string>).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string))
        {
          return (property.GetValue(attribute) as IEnumerable<string>)?.ToArray();
        }
      }

      var field = attribute.GetType().GetField(memberName, flags);
      if (field != null)
      {
        if (field.FieldType == typeof(string[]))
        {
          return field.GetValue(attribute) as string[];
        }

        if (typeof(IEnumerable<string>).IsAssignableFrom(field.FieldType) && field.FieldType != typeof(string))
        {
          return (field.GetValue(attribute) as IEnumerable<string>)?.ToArray();
        }
      }

      return null;
    }
  }
}
