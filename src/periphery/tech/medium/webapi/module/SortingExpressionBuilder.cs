namespace Tmobis.Medium.WebApi;

using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using Tmobis.Core.Converters;
using Tmobis.Core.Models;

/// <summary>
/// Преобразует внешнюю строку сортировки в LINQ-операцию.
/// </summary>
public interface ISortingExpressionBuilder
{
    Func<IQueryable<T>, IOrderedQueryable<T>> Build<T>(string? sorting)
        where T : class;
}

/// <summary>
/// Строит сортировку по строке вида "name" или "-name".
/// Пустая строка сортирует по Id по убыванию.
/// </summary>
public class SortingExpressionBuilder(
    IPropertyNameConverter propertyNameConverter)
    : ISortingExpressionBuilder
{
    public Func<IQueryable<T>, IOrderedQueryable<T>> Build<T>(string? sorting)
        where T : class
    {
        var isDescending = false;
        string propertyName;

        if (string.IsNullOrWhiteSpace(sorting))
        {
            propertyName = "Id";
            isDescending = true;
        }
        else
        {
            var rawSorting = sorting.Trim();
            if (rawSorting.StartsWith('-'))
            {
                isDescending = true;
                rawSorting = rawSorting[1..];
            }

            if (string.IsNullOrWhiteSpace(rawSorting))
            {
                throw new ArgumentException("Поле сортировки не указано.");
            }

            propertyName = propertyNameConverter.GetBackendPropertyName(rawSorting);
        }

        var property = typeof(T).GetInheritedProperty(propertyName)
            ?? throw new ArgumentException(
                $"Поле сортировки \"{propertyName}\" не найдено.");

        // Параметр приходит из HTTP: навигации, коллекции и вычисляемые
        // свойства не должны доходить до LINQ-провайдера как ключ сортировки.
        if (property.GetMethod is not { IsPublic: true } ||
            property.GetIndexParameters().Length != 0 ||
            property.IsDefined(typeof(NotMappedAttribute), true) ||
            !IsSortablePropertyType(property.PropertyType))
        {
            throw new ArgumentException(
                $"Сортировка по полю \"{propertyName}\" не поддерживается.");
        }

        var methodName = isDescending
            ? nameof(Queryable.OrderByDescending)
            : nameof(Queryable.OrderBy);

        return query => ApplyOrder(query, property, methodName);
    }

    private static bool IsSortablePropertyType(Type propertyType)
    {
        var type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        return type.IsPrimitive || type.IsEnum ||
            type == typeof(string) ||
            type == typeof(decimal) ||
            type == typeof(Guid) ||
            type == typeof(DateTime) ||
            type == typeof(DateTimeOffset) ||
            type == typeof(DateOnly) ||
            type == typeof(TimeOnly) ||
            type == typeof(TimeSpan);
    }

    private static IOrderedQueryable<T> ApplyOrder<T>(
        IQueryable<T> source,
        PropertyInfo property,
        string methodName)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
        var keySelector = Expression.Lambda(propertyAccess, parameter);
        var call = Expression.Call(
            typeof(Queryable),
            methodName,
            [typeof(T), property.PropertyType],
            source.Expression,
            keySelector);

        return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
    }
}
