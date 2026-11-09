namespace Tmobis.Medium.WebApi;

using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Tmobis.Core.Entities.Filters;
using Tmobis.Core.Models;
using Tmobis.Core.Reflection;

/// <summary>
/// Строит выражение фильтрации сущности по набору операций фильтра.
/// Операции объединяются через AND, поддерживаются пути к свойствам
/// через связанные сущности и приведение строкового значения к типу конечного свойства.
/// </summary>
public interface IEntityFilterExpressionBuilder
{
    /// <summary>
    /// Строит итоговый предикат для фильтрации по всем операциям.
    /// </summary>
    Expression<Func<T, bool>> Build<T>(EntityQueryFilter filter)
        where T : class, IEntity;
}

/// <summary>
/// Строит выражения фильтрации сущностей по операциям,
/// полученным из параметров WebAPI-запроса.
/// </summary>
public class EntityFilterExpressionBuilder : IEntityFilterExpressionBuilder
{
    private static readonly MethodInfo ContainsMethod =
        typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])
        ?? throw new InvalidOperationException("Не удалось получить метод string.Contains(string).");

    public Expression<Func<T, bool>> Build<T>(EntityQueryFilter filter)
        where T : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(filter);
        if (filter.Operations.Count == 0)
            throw new ArgumentException("Список фильтров не может быть пустым.", nameof(filter));

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? body = null;

        foreach (var operation in filter.Operations)
        {
            var filterExpression = BuildOperationExpression(parameter, operation);
            body = body == null ? filterExpression : Expression.AndAlso(body, filterExpression);
        }

        return Expression.Lambda<Func<T, bool>>(body ?? Expression.Constant(true), parameter);
    }

    /// <summary>
    /// Строит выражение фильтрации для одной операции.
    /// </summary>
    protected virtual Expression BuildOperationExpression(ParameterExpression parameter, EntityQueryFilterOperation operation)
    {
        var propertyPath = operation.PropertyName;
        var propertyPathAccess = PropertyExtractor.ExtractPropertyPathAccess(parameter, propertyPath);
        var propertyAccess = propertyPathAccess.AccessExpression;

        if (propertyAccess is not MemberExpression memberAccess)
        {
            throw new InvalidOperationException(
                $"Фильтрация по пути \"{propertyPath}\" должна завершаться конечным свойством.");
        }

        var rawValue = operation.Value?.ToString() ?? string.Empty;

        var operationExpression = operation.Operator switch
        {
            FilterOperation.Equal => BuildEqualityExpression(memberAccess, rawValue, propertyPath, isEqual: true),
            FilterOperation.NotEqual => BuildEqualityExpression(memberAccess, rawValue, propertyPath, isEqual: false),
            FilterOperation.Contains => BuildContainsExpression(memberAccess, rawValue, propertyPath),
            FilterOperation.GreaterThanOrEqual => BuildComparisonExpression(
                memberAccess, rawValue, propertyPath, ExpressionType.GreaterThanOrEqual),
            FilterOperation.LesserThanOrEqual => BuildComparisonExpression(
                memberAccess, rawValue, propertyPath, ExpressionType.LessThanOrEqual),
            FilterOperation.GreaterThan => BuildComparisonExpression(
                memberAccess, rawValue, propertyPath, ExpressionType.GreaterThan),
            FilterOperation.LesserThan => BuildComparisonExpression(
                memberAccess, rawValue, propertyPath, ExpressionType.LessThan),
            null => throw new ArgumentException($"Для свойства \"{propertyPath}\" не указан оператор фильтра."),
            _ => throw new ArgumentException(
                $"Оператор \"{operation.Operator}\" для свойства \"{propertyPath}\" пока не поддерживается.")
        };

        var nullChecks = propertyPathAccess.NullChecks
            .Aggregate((Expression?)null,
                (acc, nullCheck) => acc == null ? nullCheck : Expression.AndAlso(acc, nullCheck));

        return nullChecks == null
            ? operationExpression
            : Expression.AndAlso(nullChecks, operationExpression);
    }

    /// <summary>
    /// Строит выражение contains для строкового свойства.
    /// </summary>
    protected virtual Expression BuildContainsExpression(
        MemberExpression memberAccess,
        string value,
        string propertyPath)
    {
        if (memberAccess.Type != typeof(string))
        {
            throw new InvalidOperationException(
                $"Оператор contains поддерживается только для string. Свойство \"{propertyPath}\" имеет тип \"{memberAccess.Type.Name}\".");
        }

        var valueExpression = Expression.Constant(value, typeof(string));
        return Expression.Call(memberAccess, ContainsMethod, valueExpression);
    }

    /// <summary>
    /// Строит выражение сравнения для одного свойства.
    /// </summary>
    protected virtual Expression BuildComparisonExpression(
        MemberExpression memberAccess,
        string rawValue,
        string propertyPath,
        ExpressionType comparisonType)
    {
        var propertyType = memberAccess.Type;
        var targetType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        var convertedValue = ConvertValue(rawValue, propertyType, propertyPath);

        Expression constant = Expression.Constant(convertedValue, targetType);
        if (propertyType != targetType)
        {
            constant = Expression.Convert(constant, propertyType);
        }

        return Expression.MakeBinary(comparisonType, memberAccess, constant);
    }

    /// <summary>
    /// Строит выражение равенства или неравенства для одного свойства.
    /// </summary>
    protected virtual Expression BuildEqualityExpression(
        MemberExpression memberAccess,
        string rawValue,
        string propertyPath,
        bool isEqual)
    {
        var propertyType = memberAccess.Type;
        var targetType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        var convertedValue = ConvertValue(rawValue, propertyType, propertyPath);

        Expression constant = Expression.Constant(convertedValue, targetType);
        if (propertyType != targetType)
        {
            constant = Expression.Convert(constant, propertyType);
        }

        return isEqual
            ? Expression.Equal(memberAccess, constant)
            : Expression.NotEqual(memberAccess, constant);
    }

    /// <summary>
    /// Преобразует строковое значение фильтра к типу конечного свойства.
    /// </summary>
    protected virtual object ConvertValue(string rawValue, Type propertyType, string propertyPath)
    {
        var targetType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        if (targetType.IsEnum)
            return ParseEnumValue(rawValue, targetType, propertyPath);

        try
        {
            if (targetType == typeof(string)) return rawValue;
            if (targetType == typeof(Guid)) return Guid.Parse(rawValue);
            if (targetType == typeof(bool)) return bool.Parse(rawValue);
            if (targetType == typeof(int)) return int.Parse(rawValue, CultureInfo.InvariantCulture);
            if (targetType == typeof(long)) return long.Parse(rawValue, CultureInfo.InvariantCulture);
            if (targetType == typeof(decimal)) return decimal.Parse(rawValue, CultureInfo.InvariantCulture);
            if (targetType == typeof(DateTime))
            {
                return DateTime.Parse(rawValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }

            if (targetType == typeof(DateOnly))
            {
                return ParseDateOnlyValue(rawValue, propertyPath);
            }

        }
        catch (Exception ex) when (ex is FormatException or OverflowException)
        {
            throw new ArgumentException(
                $"Не удалось преобразовать значение \"{rawValue}\" к типу \"{targetType.Name}\" для свойства \"{propertyPath}\".", ex);
        }

        throw new NotSupportedException(
            $"Тип \"{targetType.Name}\" для свойства \"{propertyPath}\" не поддерживается фильтрацией.");
    }

    /// <summary>
    /// Преобразует строковое значение фильтра к DateOnly.
    /// </summary>
    protected virtual DateOnly ParseDateOnlyValue(string rawValue, string propertyPath)
    {
        if (DateOnly.TryParse(rawValue, CultureInfo.InvariantCulture, out var dateOnly))
        {
            return dateOnly;
        }

        if (DateTime.TryParse(rawValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dateTime))
        {
            return DateOnly.FromDateTime(dateTime);
        }

        throw new ArgumentException(
            $"Не удалось преобразовать значение \"{rawValue}\" к типу \"DateOnly\" для свойства \"{propertyPath}\".");
    }

    /// <summary>
    /// Преобразует строковое имя значения перечисления к enum-значению.
    /// </summary>
    protected virtual object ParseEnumValue(string rawValue, Type enumType, string propertyPath)
    {
        var enumNames = Enum.GetNames(enumType);
        if (!enumNames.Contains(rawValue))
        {
            throw new ArgumentException(
                $"Значение \"{rawValue}\" не является допустимым значением перечисления для свойства \"{propertyPath}\".");
        }

        return Enum.Parse(enumType, rawValue);
    }
}
