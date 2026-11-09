using System.Reflection;
using Scriban;
using Scriban.Runtime;

if (args.Length == 0 || args[0] != "generate")
{
    Console.WriteLine("Использование: entitygen generate --assembly <путь> [--output <каталог>]");
    return 1;
}

var assemblyPath = GetArg(args, "--assembly") ?? throw new ArgumentException("--assembly является обязательным");
var outputDir = GetArg(args, "--output") ?? "./Entities/Generated";

Directory.CreateDirectory(outputDir);

var assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
var directory = Path.GetDirectoryName(assemblyPath)!;
var dllPaths = Directory.GetFiles(directory, "*.dll");
var tmobisCorePath = Path.Combine(directory, "tmobis.core.dll");

// Загрузить сборку Tmobis.Core первой для получения базовых типов
Assembly tmobisCore;
using var mlc = CreateLoadContext(assemblyPath);

try
{
    tmobisCore = mlc.LoadFromAssemblyPath(tmobisCorePath);
}
catch (Exception ex)
{
    Console.Error.WriteLine("Не удалось загрузить сборку Tmobis.Core: {0}", ex);
    return 1;
}

// Получить тип IEntity из Tmobis.Core
var entityInterface = tmobisCore.GetTypes()
    .FirstOrDefault(t => t is { IsInterface: true, Name: "IEntity" });

if (entityInterface == null)
{
    Console.Error.WriteLine("Не удалось найти интерфейс IEntity в Tmobis.Core");
    return 1;
}

// Получить интерфейс IRegisteredEntity из Tmobis.Core
var registeredEntityInterface = tmobisCore.GetTypes()
    .FirstOrDefault(t => t is { IsInterface: true, Name: "IRegisteredEntity" });

// Получить класс BusinessEntity из Tmobis.Core
var entityClass = tmobisCore.GetTypes()
    .FirstOrDefault(t => t is { IsInterface: false, Name: "BusinessEntity" });

// Найти все интерфейсы во всех сборках, которые реализуют IRegisteredEntity
var entityInterfaces = new List<Type>();
foreach (var path in dllPaths)
{
    try
    {
        var asm = mlc.LoadFromAssemblyPath(path);
        var types = asm.GetTypes();
        entityInterfaces.AddRange(
            types.Where(t => t.IsInterface && t.GetInterfaces().Any(i => i == registeredEntityInterface)));
    }
    catch
    {
        // Пропустить сборки, которые не удалось загрузить
    }
}

if (entityInterfaces.Count == 0)
{
    Console.WriteLine("Не найдено интерфейсов, производных от IEntity.");
    return 0;
}

// Поиск "прямых" интерфейсов - которые не наследуются от других интерфейсов
static IEnumerable<Type> GetDirectInterfaces(Type iface)
{
    var all = iface.GetInterfaces();
    return all.Where(candidate =>
        !all.Any(other => other != candidate && other.GetInterfaces().Contains(candidate)));
}

// Поиск прямого базового IEntity, который реализует IRegisteredEntity
Type? GetRegisteredEntityBase(Type iface)
{
    // Реализует ли интерфейс напрямую IRegisteredEntity
    if (GetDirectInterfaces(iface).Any(i => i == registeredEntityInterface))
    {
        return iface;
    }

    // Поиск наследника IEntity, который реализует IRegisteredEntity
    var interfaces = iface.GetInterfaces()
        .Where(i => i == entityInterface || i.GetInterfaces().Any(p => p == entityInterface)).ToList();

    foreach (var @interface in interfaces)
    {
        if (@interface.GetInterfaces().Any(i => i == registeredEntityInterface))
        {
            return @interface;
        }
    }

    return null;
}

// Группировка реализаций IRegisteredEntity по которым будут сгенерированы сущности
var grouped = new Dictionary<Type, List<Type>>();
foreach (var iface in entityInterfaces)
{
    var baseRegistered = GetRegisteredEntityBase(iface);
    if (baseRegistered == null)
    {
        continue;
    }

    // Группировка сущностей по прямым реализациям IRegisteredEntity
    if (!grouped.ContainsKey(baseRegistered))
    {
        grouped[baseRegistered] = [];
    }

    // В список интерфейсов генерируемой сущности добавляются все реализуемые интерфейс-сущности
    if (!grouped[baseRegistered].Contains(iface))
    {
        grouped[baseRegistered].Add(iface);
    }

    var implementedInterfaces = iface.GetInterfaces().Where(i => entityInterfaces.Contains(i)).ToList();
    foreach (var implemented in implementedInterfaces)
    {
        if (!grouped[baseRegistered].Contains(implemented))
        {
            grouped[baseRegistered].Add(implemented);
        }
    }
}

if (grouped.Count == 0)
{
    Console.WriteLine("Не найдено интерфейсов, реализующих IRegisteredEntity.");
    return 0;
}

var allInterfaces = grouped.Values.SelectMany(i => i).Distinct().ToList();
var nameMap = new Dictionary<Type, string>();
foreach (var iface in allInterfaces)
{
    Type groupKey = iface;
    if (!iface.GetInterfaces().Any(i => i == registeredEntityInterface))
    {
        var baseRegistered = GetRegisteredEntityBase(iface);
        groupKey = baseRegistered ?? iface;
    }

    nameMap[iface] = ToEntityName(groupKey);
}

var entityTemplate = Template.Parse(EmbeddedTemplates.Entity);
var errors = new List<string>();
// Генерация сущностей по группировкам интерфейсов
foreach (var kvp in grouped)
{
    var groupKey = kvp.Key;
    var interfaces = kvp.Value;

    try
    {
        var model = ClassifyEntityGroup(
            interfaces, nameMap, entityInterface,
            registeredEntityInterface, tmobisCore, entityClass, groupKey);
        WriteEntity(model, outputDir, entityTemplate, assemblyName, nameMap, entityInterface,
            registeredEntityInterface);
        Console.WriteLine($"Сгенерирован {model.Name}.g.cs");
    }
    catch (Exception ex)
    {
        var names = string.Join(", ", interfaces.Select(i => i.Name));
        errors.Add($"[{names}]: {ex.Message}");
    }
}

if (errors.Count > 0)
{
    Console.Error.WriteLine("Генерация завершилась с ошибками:");
    foreach (var e in errors) Console.Error.WriteLine($"  - {e}");
    return 1;
}

return 0;

static string? GetArg(string[] args, string name)
{
    var idx = Array.IndexOf(args, name);
    return idx >= 0 && idx + 1 < args.Length ? args[idx + 1] : null;
}

static MetadataLoadContext CreateLoadContext(string assemblyPath)
{
    var dir = Path.GetDirectoryName(assemblyPath)!;
    var deps = Directory.GetFiles(dir, "*.dll");
    var trustedAssemblies = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
        .Split(Path.PathSeparator);
    var resolver = new PathAssemblyResolver(deps.Concat(trustedAssemblies).Distinct());
    return new MetadataLoadContext(resolver, coreAssemblyName: "System.Private.CoreLib");
}

static string ToEntityName(Type iface) => iface.Name.TrimStart('I');

static List<PropertyInfo> GetAllInterfaceProperties(Type iface)
{
    var all = new Dictionary<string, PropertyInfo>();

    void Collect(Type t)
    {
        foreach (var p in t.GetProperties())
            all.TryAdd(p.Name, p);
        foreach (var i in t.GetInterfaces())
            Collect(i);
    }

    Collect(iface);
    return all.Values.ToList();
}

// Проверяет, имеет ли свойство атрибут ComputedProperty и возвращает имя метода
static string? GetGeneratedMethodName(PropertyInfo prop)
{
    var attr = prop.GetCustomAttributesData()
        .FirstOrDefault(a => a.AttributeType.Name == "ComputedPropertyAttribute");
    if (attr is null)
        return null;

    // Получаем имя метода из аргумента атрибута
    if (attr.ConstructorArguments.Count > 0)
    {
        var arg = attr.ConstructorArguments[0];
        if (arg.Value is string methodName)
        {
            return methodName;
        }
    }

    return null;
}

// Типы MetadataLoadContext не могут использовать NullabilityInfoContext (требуется
// загруженный во время выполнения Type). Метаданные Nullable reference закодированы через
// атрибуты [Nullable] / [NullableContext], выдаваемые компилятором.
static bool IsNullableRef(PropertyInfo prop)
{
    if (prop.PropertyType.IsValueType) return false; // обрабатывается отдельно для Nullable<T>

    var nullableAttr = prop.GetCustomAttributesData()
        .FirstOrDefault(a => a.AttributeType.Name == "NullableAttribute");
    if (nullableAttr is not null)
    {
        var arg = nullableAttr.ConstructorArguments.FirstOrDefault();
        if (arg.Value is byte b) return b == 2; // 1 = не nullable, 2 = nullable
        if (arg.Value is System.Collections.ObjectModel.ReadOnlyCollection<CustomAttributeTypedArgument> arr
            && arr.Count > 0 && arr[0].Value is byte b0)
            return b0 == 2;
    }

    var ctxAttr = prop.DeclaringType!.GetCustomAttributesData()
        .FirstOrDefault(a => a.AttributeType.Name == "NullableContextAttribute");
    if (ctxAttr?.ConstructorArguments.FirstOrDefault().Value is byte ctxByte)
        return ctxByte == 2;

    return false;
}

static bool IsNullableValueType(PropertyInfo prop) =>
    prop.PropertyType.IsGenericType &&
    prop.PropertyType.GetGenericTypeDefinition().Name == "Nullable`1";

static string FormatTypeName(PropertyInfo prop)
{
    var type = prop.PropertyType;

    // Обработка Nullable<T> -> T?
    if (type.IsGenericType &&
        type.GetGenericTypeDefinition().Name == "Nullable`1")
    {
        var arg = type.GetGenericArguments()[0];
        return GetFullTypeName(arg) + "?";
    }

    return GetFullTypeName(type);
}

static string GetFullTypeName(Type type)
{
    if (type.IsGenericType && type.GetGenericTypeDefinition().Name == "Nullable`1")
    {
        var arg = type.GetGenericArguments()[0];
        return GetFullTypeName(arg) + "?";
    }

    if (type.IsGenericType)
    {
        var genericTypeDef = type.GetGenericTypeDefinition();
        var genericArgs = type.GetGenericArguments();
        var genericArgsString = string.Join(", ", genericArgs.Select(GetFullTypeName));
        var backtickIndex = genericTypeDef.Name.IndexOf('`');
        var baseName = backtickIndex >= 0 ? genericTypeDef.Name.Substring(0, backtickIndex) : genericTypeDef.Name;
        return $"{genericTypeDef.Namespace}.{baseName}<{genericArgsString}>";
    }

    if (type.IsArray)
    {
        var elementType = type.GetElementType();
        return GetFullTypeName(elementType!) + "[]";
    }

    // Для указательных типов
    if (type.IsPointer)
    {
        var elementType = type.GetElementType();
        return GetFullTypeName(elementType!) + "*";
    }

    // Для byref типов
    if (type.IsByRef)
    {
        var elementType = type.GetElementType();
        return GetFullTypeName(elementType!) + "&";
    }

    // Для всех остальных типов (включая типы значений, перечисления, примитивы)
    // Обработка типов без пространства имен (например, void, указательные типы без элемента)
    var ns = type.Namespace ?? string.Empty;
    return string.IsNullOrEmpty(ns) ? type.Name : $"{ns}.{type.Name}";
}

static void CollectBaseClassProperties(
    Type iface, Dictionary<string, PropertyInfo> baseProps, HashSet<Type> visited)
{
    if (visited.Contains(iface)) return;
    visited.Add(iface);

    foreach (var prop in iface.GetProperties())
    {
        baseProps.TryAdd(prop.Name, prop);
    }

    foreach (var baseInterface in iface.GetInterfaces())
    {
        CollectBaseClassProperties(baseInterface, baseProps, visited);
    }
}

static List<string> GetAnnotationAttributeStrings(PropertyInfo prop)
{
    var blocked = new[] { "Nullable" };
    var result = new List<string>();

    foreach (var attr in prop.GetCustomAttributesData())
    {
        if (blocked.Contains(attr.AttributeType.Name.Replace("Attribute", "")))
            continue;

        var args = attr.ConstructorArguments.Select(FormatArg);
        var attributeName = attr.AttributeType.Name.Replace("Attribute", "");
        var attributeNamespace = attr.AttributeType.Namespace;
        var name = string.IsNullOrEmpty(attributeNamespace) ? attributeName : $"{attributeNamespace}.{attributeName}";
        result.Add(args.Any() ? $"{name}({string.Join(", ", args)})" : name);
    }

    return result;
}

static string FormatArg(CustomAttributeTypedArgument arg)
{
    if (arg.Value == null)
        return "null";

    // Обработка массивов (для ArgumentType)
    if (arg.Value is System.Collections.IEnumerable enumerable && arg.ArgumentType.IsArray)
    {
        var elements = new List<string>();
        foreach (var item in enumerable)
        {
            elements.Add(FormatArg(new CustomAttributeTypedArgument(arg.ArgumentType.GetElementType()!, item)));
        }
        return "{" + string.Join(", ", elements) + "}";
    }

    // Обработка типов (typeof)
    if (arg.Value is Type type)
    {
        return $"typeof({GetFullTypeName(type)})";
    }

    // Обработка строк
    if (arg.Value is string s)
        return $"\"{s}\"";

    // Обработка enum
    // MetadataLoadContext возвращает enum как int, поэтому проверяем ArgumentType
    if (arg.ArgumentType.IsEnum)
    {
        // Пытаемся получить имя enum через Enum.GetName
        // arg.Value может быть int или string (если это было передано как string)
        var enumValue = arg.Value;
        var enumType = arg.ArgumentType;

        // Получаем имя enum члена
        var enumName = GetEnumName(enumType, enumValue);
        if (enumName != null)
        {
            return $"{GetFullTypeName(enumType)}.{enumName}";
        }
        // Если не удалось получить имя, используем числовое значение
        return enumValue.ToString()!;
    }

// Получить имя enum члена по значению (для MetadataLoadContext)
static string? GetEnumName(Type enumType, object enumValue)
{
    // Пытаемся получить имя через Enum.GetName
    // Для MetadataLoadContext enumValue может быть int или string
    try
    {
        // Если это int, пытаемся найти имя
        if (enumValue is int intValue)
        {
            return Enum.GetName(enumType, intValue);
        }
        // Если это string, возможно это уже имя
        if (enumValue is string stringValue)
        {
            // Проверяем, является ли это числом
            if (int.TryParse(stringValue, out _))
            {
                return Enum.GetName(enumType, int.Parse(stringValue));
            }
            return stringValue;
        }
    }
    catch
    {
        // Игнорируем ошибки
    }
    return null;
}

    // Обработка примитивов и других типов
    return arg.Value.ToString()!;
}

static EntityModel ClassifyEntityGroup(List<Type> interfaces, Dictionary<Type, string> nameMap, Type entityInterface,
    Type? registeredEntityInterface, Assembly tmobisCore, Type entityClass, Type groupKey)
{
    var allProps = new Dictionary<string, PropertyInfo>();
    var propSource = new Dictionary<string, Type>();

    foreach (var iface in interfaces)
    {
        var props = GetAllInterfaceProperties(iface);
        foreach (var prop in props)
        {
            if (!allProps.ContainsKey(prop.Name))
            {
                allProps[prop.Name] = prop;
                propSource[prop.Name] = iface;
            }
        }
    }

    var scalars = new List<ScalarProp>();
    var computedProps = new List<ComputedProp>();

    var baseProps = new Dictionary<string, PropertyInfo>();
    var visited = new HashSet<Type>();

    // Поиск интерфейса, который можно замаппить в базовый класс
    Type? rootWithBaseClass = null;
    foreach (var iface in interfaces)
    {
        var ifaceBaseClass = GetBaseClassNameFromInterface(iface.Name);
        if (!string.IsNullOrEmpty(ifaceBaseClass))
        {
            rootWithBaseClass = iface;
            break;
        }

        foreach (var baseIf in iface.GetInterfaces())
        {
            var baseIfBaseClass = GetBaseClassNameFromInterface(baseIf.Name);
            if (!string.IsNullOrEmpty(baseIfBaseClass))
            {
                rootWithBaseClass = baseIf;
                break;
            }
        }

        if (rootWithBaseClass != null) break;
    }

    Type? baseClassType = null;
    // Загрузка базового типа класса из Tmobis.Core
    if (rootWithBaseClass != null && tmobisCore != null)
    {
        var mappedBaseClass = GetBaseClassNameFromInterface(rootWithBaseClass.Name);
        if (!string.IsNullOrEmpty(mappedBaseClass))
        {
            baseClassType = tmobisCore.GetTypes().FirstOrDefault(t => t.Name == mappedBaseClass && !t.IsInterface);
        }
    }

    if (rootWithBaseClass == null)
    {
        baseClassType = entityClass;
    }

    if (baseClassType != null)
    {
        CollectBaseClassProperties(baseClassType, baseProps, visited);
    }

    foreach (var prop in allProps.Values)
    {
        // Пропуск свойств, которые существуют в базовых классах
        if (baseProps.ContainsKey(prop.Name))
        {
            continue;
        }
        
        // Свойства с атрибутом ComputedProperty обрабатываются отдельно
        var methodName = GetGeneratedMethodName(prop);
        if (methodName != null)
        {
            computedProps.Add(new ComputedProp(
                prop.Name,
                FormatTypeName(prop),
                methodName,
                GetAnnotationAttributeStrings(prop)));
        }
        else
        {
            scalars.Add(new ScalarProp(
                prop.Name,
                GetFullTypeName(prop.PropertyType),
                FormatTypeName(prop),
                IsNullableRef(prop) || IsNullableValueType(prop),
                GetAnnotationAttributeStrings(prop)));
        }
    }

    var implementedInterfaces = new HashSet<string>();
    foreach (var iface in interfaces)
    {
        implementedInterfaces.Add(iface.Name);
    }

    var fullNames = interfaces.Select(i => $"{i.Namespace}.{i.Name}").ToList();

    var rootInterface = groupKey;
    var rootInterfaceName = rootInterface.Name;

    string? baseClassName = null;
    Type? baseInterface = null;

    var baseClassNameShort = GetBaseClassNameFromInterface(rootInterfaceName);
    if (!string.IsNullOrEmpty(baseClassNameShort))
    {
        baseInterface = rootInterface;
    }

    if (string.IsNullOrEmpty(baseClassNameShort))
    {
        var allInterfacesInGroup = new HashSet<Type>();
        foreach (var iface in interfaces)
        {
            allInterfacesInGroup.Add(iface);
            foreach (var @interface in iface.GetInterfaces())
            {
                allInterfacesInGroup.Add(@interface);
            }
        }

        foreach (var iface in allInterfacesInGroup)
        {
            var ifaceBaseClass = GetBaseClassNameFromInterface(iface.Name);
            if (!string.IsNullOrEmpty(ifaceBaseClass))
            {
                baseClassNameShort = ifaceBaseClass;
                baseInterface = iface;
                break;
            }
        }
    }

    var hasBaseClassMapping = !string.IsNullOrEmpty(baseClassNameShort);

    if (!hasBaseClassMapping)
    {
        hasBaseClassMapping = true;
        baseClassNameShort = "BusinessEntity";
        baseInterface = rootInterface;
    }

    if (hasBaseClassMapping && tmobisCore != null)
    {
        var baseClassTypeModel = tmobisCore.GetTypes().FirstOrDefault(t => t.Name == baseClassNameShort && !t.IsInterface);
        if (baseClassTypeModel != null)
        {
            baseClassName = $"{baseClassTypeModel.Namespace}.{baseClassTypeModel.Name}";
        }
        else
        {
            baseClassName = baseClassNameShort;
        }
    }
    else
    {
        baseClassName = baseClassNameShort;
    }

    // Сбор атрибутов интерфейсов для всех интерфейсов группы
    var interfaceAttributes = new List<string>();
    var visitedInterfaces = new HashSet<Type>();
    void CollectInterfaceAttributes(Type iface)
    {
        if (visitedInterfaces.Contains(iface)) return;
        visitedInterfaces.Add(iface);

        var attrs = GetInterfaceAttributes(iface);
        foreach (var attr in attrs)
        {
            var args = attr.ConstructorArguments.Select(FormatArg);
            var attributeName = attr.AttributeType.Name.Replace("Attribute", "");
            var attributeNamespace = attr.AttributeType.Namespace;
            var name = string.IsNullOrEmpty(attributeNamespace) ? attributeName : $"{attributeNamespace}.{attributeName}";
            var formatted = args.Any() ? $"{name}({string.Join(", ", args)})" : name;
            if (!interfaceAttributes.Contains(formatted))
            {
                interfaceAttributes.Add(formatted);
            }
        }

        // Рекурсивно обрабатываем базовые интерфейсы
        foreach (var baseIf in iface.GetInterfaces())
        {
            CollectInterfaceAttributes(baseIf);
        }
    }

    foreach (var iface in interfaces)
    {
        CollectInterfaceAttributes(iface);
    }

    return new EntityModel(nameMap[groupKey], fullNames, scalars, computedProps,
        implementedInterfaces.ToList(), baseClassName, baseInterface!.Name, hasBaseClassMapping, interfaceAttributes);
}

// Построение списка наследования
static string BuildImplementsString(EntityModel model, Dictionary<Type, string> nameMap, Type entityInterface,
    Type? registeredEntityInterface)
{
    // BusinessEntity используется в качестве базового класса по умолчанию
    var parts = new List<string> { model.BaseClassName };

    foreach (var fullName in model.FullNames)
    {
        var interfaceName = fullName.Split('.').Last();

        if (interfaceName == "IEntity")
        {
            parts.Add(interfaceName);
        }
        else
        {
            parts.Add(fullName);
        }
    }

    return string.Join(", ", parts);
}

// Получение базового класса по интерфейсу
static string GetBaseClassNameFromInterface(string interfaceName)
{
    if (interfaceName == "IActualEntity")
        return "ActualEntity";
    if (interfaceName == "IDictionary")
        return "Dictionary";
    return string.Empty;
}

static void WriteEntity(EntityModel model, string outputDir, Template template, string assemblyName,
    Dictionary<Type, string> nameMap, Type entityInterface, Type? registeredEntityInterface)
{
    var scriptObj = new ScriptObject();
    scriptObj.Import(model, renamer: m => ToSnakeCase(m.Name));
    scriptObj["namespace"] = assemblyName;
    scriptObj["inheritance"] = BuildImplementsString(model, nameMap, entityInterface, registeredEntityInterface);

    var context = new TemplateContext();
    context.PushGlobal(scriptObj);
    var rendered = template.Render(context);
    File.WriteAllText(Path.Combine(outputDir, $"{model.Name}.g.cs"), rendered);
}

static string ToSnakeCase(string s) =>
    string.Concat(s.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c : c.ToString())).ToLower();

// Получить атрибуты интерфейса, которые можно применять к классам
#nullable disable
static List<CustomAttributeData> GetInterfaceAttributes(Type iface)
{
    var result = new List<CustomAttributeData>();

    foreach (var attr in iface.GetCustomAttributesData())
    {
        var attrType = attr.AttributeType;

        // Пропускаем системные атрибуты (из System.* namespace)
        if (attrType.Namespace == "System" || attrType.Namespace.StartsWith("System."))
        {
            continue;
        }
#nullable restore

        // Проверяем, можно ли применять атрибут к классам через AttributeUsage
        var attrUsage = attrType.GetCustomAttributesData()
            .FirstOrDefault(a => a.AttributeType.Name == "AttributeUsageAttribute");

        if (attrUsage == null)
        {
            // Если нет AttributeUsage, копируем атрибут (консервативный подход)
            // Это нужно для атрибутов, у которых Target не определен явно
            result.Add(attr);
            continue;
        }

        // Анализируем Target свойство AttributeUsage
        // Target - это поле, которое содержит значение перечисления AttributeTargets
        var targetField = attrUsage.AttributeType.GetField("Target", System.Reflection.BindingFlags.Public | BindingFlags.Static);
        if (targetField != null)
        {
            var targetValue = targetField.GetValue(null);
            // AttributeTargets.Class = 4
            var canApplyToClass = (targetValue is int targets) && (targets & 4) == 4;
            if (canApplyToClass)
            {
                result.Add(attr);
            }
        }
        else
        {
            // Если не удалось найти Target, копируем атрибут (консервативный подход)
            result.Add(attr);
        }
    }

    return result;
}

record EntityModel(
    string Name,
    List<string> FullNames,
    List<ScalarProp> Scalars,
    List<ComputedProp> ComputedProps,
    List<string> Implements,
    string BaseClassName,
    string RootInterfaceName,
    bool HasBaseClassMapping,
    List<string> InterfaceAttributes);

record ScalarProp(string Name, string TypeName, string TypeString, bool Nullable, List<string> AttributeSources);

record ComputedProp(
    string Name,
    string TypeString,
    string? GeneratedMethodName,
    List<string> AttributeSources);

static class EmbeddedTemplates
{
    public const string Entity = """
                                 // <auto-generated />
                                 namespace {{ namespace }};

                                 {{~ for attr in interface_attributes ~}}
                                 [{{ attr }}]
                                 {{~ end ~}}
                                 public partial class {{ name }} : {{ inheritance }}
                                 {
                                     {{~ for s in scalars ~}}
                                     {{~ for attr in s.attribute_sources ~}}
                                     [{{ attr }}]
                                     {{~ end ~}}
                                     public virtual {{ s.type_string }} {{ s.name }} { get; set; }

                                     {{~ end ~}}
                                     {{~ for n in computed_props ~}}
                                     {{~ for attr in n.attribute_sources ~}}
                                     [{{ attr }}]
                                     {{~ end ~}}
                                     {{~ if n.generated_method_name ~}}
                                     public virtual {{ n.type_string }} {{ n.name }} => {{ n.generated_method_name }}(this);

                                     {{~ else ~}}
                                     public virtual {{ n.type_string }} {{ n.name }} { get; }

                                     {{~ end ~}}
                                     {{~ end ~}}
                                 }
                                 """;
}