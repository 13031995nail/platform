namespace Tmobis.Core.Reflection;

using System.Linq.Expressions;
using System.Reflection;
using Models;

/// <summary>
/// Содержит вспомогательные методы для работы с путями свойств:
/// получение пути из лямбда-выражения, обход свойств объекта
/// и построение выражения доступа к свойству по строковому пути.
/// </summary>
public static class PropertyExtractor
{
    /// <summary>
    /// Рекурсивный метод получения пути для свойства, заданного через вложенную лямбду.
    /// Лямбда-выражение может содержать вложенные обращения к мастерам.
    /// 
    /// </summary>
    /// <typeparam name="TProperty">Тип свойства.</typeparam>
    /// <param name="propertyExpression">Лямбда-выражение для доступа к свойству.</param>
    /// <returns>Полный путь к свойству (разделение через точку).</returns>
    /// <seealso href="https://github.com/Flexberry/NewPlatform.Flexberry.ORM/blob/develop/ICSSoft.STORMNET.DataObject/Information.cs"/>
    public static string Path<TProperty>(
        Expression<Func<TProperty, object>> propertyExpression)
    => InternalExtractPropertyPath(ExtractMemberExpression(propertyExpression));

    /// <summary>
    /// Метод получения MemberExpression из лямбда-выражения.
    /// </summary>
    /// <param name="propertyExpression">Выражение для получения значения свойства.</param>
    /// <returns>System.Linq.Expressions.MemberExpression для получения значения.</returns>
    /// <seealso href="https://github.com/Flexberry/NewPlatform.Flexberry.ORM/blob/develop/ICSSoft.STORMNET.DataObject/Information.cs"/>
    private static MemberExpression ExtractMemberExpression(
        LambdaExpression propertyExpression)
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);

        Expression body = propertyExpression.Body;
        MemberExpression? memberExpression = body as MemberExpression;
        if (memberExpression == null && (body.NodeType == ExpressionType.Convert || body.NodeType == ExpressionType.ConvertChecked))
        {
            memberExpression = ((UnaryExpression)body).Operand as MemberExpression;
        }

        if (memberExpression == null)
        {
            throw new ArgumentException("Выражение не является членом класса", nameof(propertyExpression));
        }

        return memberExpression;
    }

    /// <summary>
    /// Рекурсивный метод получения пути для свойства, заданного через вложенную лямбду.
    /// Лямбда-выражение может содержать вложенные обращения к мастерам.
    /// </summary>
    /// <param name="propertyExpression">Лямбда - выражение для доступа к свойству.</param>
    /// <returns>Полный путь к свойству (разделение через точку).</returns>
    /// <seealso href="https://github.com/Flexberry/NewPlatform.Flexberry.ORM/blob/develop/ICSSoft.STORMNET.DataObject/Information.cs"/>
    private static string? InternalExtractPropertyPath(MemberExpression propertyExpression)
    {
        if (propertyExpression == null)
        {
            return null;
        }

        string name = propertyExpression.Member.Name;
        MemberExpression? memberExpression = propertyExpression.Expression as MemberExpression;
        if (memberExpression == null)
        {
            throw new ArgumentException("Выражение не является членом класса", nameof(propertyExpression));
        }
        string? text = InternalExtractPropertyPath(memberExpression);
        if (!string.IsNullOrEmpty(text))
        {
            return $"{text}.{name}";
        }

        return name;
    }

    /// <summary>
    /// Результат построения доступа к свойству по строковому пути.
    /// </summary>
    /// <param name="AccessExpression">Выражение доступа к конечному свойству.</param>
    /// <param name="NullChecks">Проверки на null для безопасного доступа к пути.</param>
    public sealed record PropertyPathAccessResult(
        Expression AccessExpression,
        IReadOnlyList<Expression> NullChecks);

    /// <summary>
    /// Строит выражение доступа к свойству по строковому пути и набор null-проверок.
    /// </summary>
    /// <param name="rootExpression">Корневое выражение (например, параметр x).</param>
    /// <param name="propertyPath">Путь свойства через точку.</param>
    /// <returns>Выражение доступа и проверки на null для каждого nullable-сегмента пути.</returns>
    public static PropertyPathAccessResult ExtractPropertyPathAccess(
        Expression rootExpression,
        string propertyPath)
    {
        ArgumentNullException.ThrowIfNull(rootExpression);

        if (string.IsNullOrWhiteSpace(propertyPath))
            throw new ArgumentException("Путь свойства не может быть пустым.", nameof(propertyPath));

        var pathParts = propertyPath.Split('.');
        if (pathParts.Length == 0 || pathParts.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException($"Путь свойства \"{propertyPath}\" некорректен.", nameof(propertyPath));

        Expression current = rootExpression;
        var nullChecks = new List<Expression>();

        foreach (var part in pathParts)
        {
            var property = current.Type.GetInheritedProperty(part);
            if (property == null)
            {
                throw new InvalidOperationException(
                    $"Путь свойства \"{propertyPath}\" некорректен. Свойство \"{part}\" не найдено в типе \"{current.Type.Name}\".");
            }

            current = Expression.Property(current, property);

            if (!current.Type.IsValueType || Nullable.GetUnderlyingType(current.Type) != null)
            {
                nullChecks.Add(Expression.NotEqual(
                    current,
                    Expression.Constant(null, current.Type)));
            }
        }

        return new PropertyPathAccessResult(current, nullChecks);
    }

    /// <summary>
    /// Строит выражение доступа к свойству по имени.
    /// </summary>
    /// <param name="rootExpression">Корневое выражение.</param>
    /// <param name="propertyName">Имя свойства.</param>
    /// <returns>Выражение доступа к свойству.</returns>
    public static Expression ExtractPropertyAccess(
        Expression rootExpression,
        string propertyName)
    {
        ArgumentNullException.ThrowIfNull(rootExpression);

        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentException("Имя свойства не может быть пустым.", nameof(propertyName));

        if (!TryExtractPropertyAccess(rootExpression, propertyName, out var accessExpression))
        {
            throw new InvalidOperationException(
                $"Свойство \"{propertyName}\" не найдено в типе \"{rootExpression.Type.Name}\".");
        }

        return accessExpression;
    }

    /// <summary>
    /// Пытается построить выражение доступа к свойству по имени.
    /// </summary>
    /// <param name="rootExpression">Корневое выражение.</param>
    /// <param name="propertyName">Имя свойства.</param>
    /// <param name="accessExpression">Выражение доступа к свойству.</param>
    /// <returns>Удалось ли построить выражение доступа.</returns>
    public static bool TryExtractPropertyAccess(
        Expression rootExpression,
        string propertyName,
        out Expression accessExpression)
    {
        ArgumentNullException.ThrowIfNull(rootExpression);

        var property = rootExpression.Type.GetInheritedProperty(propertyName);
        if (property == null)
        {
            accessExpression = null!;
            return false;
        }

        accessExpression = Expression.Property(rootExpression, property);
        return true;
    }

    /// <summary>
    /// Проверяет, является ли тип идентификатором Guid.
    /// </summary>
    /// <param name="type">Тип значения.</param>
    /// <returns>Является ли тип Guid или nullable Guid.</returns>
    public static bool IsGuidType(Type type)
        => type == typeof(Guid) || type == typeof(Guid?);

    /// <summary>
    /// Пытается построить выражение доступа к Guid-свойству.
    /// </summary>
    /// <param name="rootExpression">Корневое выражение.</param>
    /// <param name="propertyName">Имя свойства.</param>
    /// <param name="accessExpression">Выражение доступа к Guid-свойству.</param>
    /// <returns>Удалось ли построить выражение доступа.</returns>
    public static bool TryExtractGuidPropertyAccess(
        Expression rootExpression,
        string propertyName,
        out Expression accessExpression)
    {
        if (TryExtractPropertyAccess(
            rootExpression,
            propertyName,
            out accessExpression)
            && IsGuidType(accessExpression.Type))
        {
            return true;
        }

        accessExpression = null!;
        return false;
    }

    /// <summary>
    /// Пытается получить Guid-идентификатор выражения.
    /// </summary>
    /// <param name="rootExpression">Выражение объекта или Guid-значения.</param>
    /// <param name="accessExpression">Выражение Guid-идентификатора.</param>
    /// <returns>Удалось ли получить Guid-идентификатор.</returns>
    public static bool TryExtractGuidIdAccess(
        Expression rootExpression,
        out Expression accessExpression)
    {
        ArgumentNullException.ThrowIfNull(rootExpression);

        if (IsGuidType(rootExpression.Type))
        {
            accessExpression = rootExpression;
            return true;
        }

        return TryExtractGuidPropertyAccess(
            rootExpression,
            nameof(IEntity.Id),
            out accessExpression);
    }

    public static async ValueTask ForEachPropertyInPath(
        object? obj, string propPath, Func<object?, PropertyInfo, string, ValueTask> action)
    {
        if (obj == null) return;
        
        var curObj = obj;
        var propSplit = propPath.Split('.');
        foreach (var prop in propSplit)
        {
            if (curObj == null) return;
            var curProp = curObj.GetType().GetProperty(prop);
            if (curProp == null) continue;
            await (action?.Invoke(curObj, curProp, prop) ?? ValueTask.CompletedTask);
            curObj = curProp.GetValue(curObj);
        }
    }
}
