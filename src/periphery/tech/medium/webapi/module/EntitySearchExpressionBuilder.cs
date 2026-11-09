namespace Tmobis.Medium.WebApi;

using System.Linq.Expressions;
using System.Reflection;
using Tmobis.Core.Models;
using Tmobis.Core.Reflection;

/// <summary>
/// Строит выражение поиска по строковым свойствам сущности,
/// указанным в представлении.
/// </summary>
public interface IEntitySearchExpressionBuilder
{
    /// <summary>
    /// Создает условие вида:
    /// свойство1 содержит строку поиска ИЛИ свойство2 содержит строку поиска.
    /// Поддерживает пути к свойствам связанных сущностей через точку.
    /// </summary>
    Expression<Func<T, bool>> Build<T>(
        IReadOnlySet<string> searchProperties,
        string search)
    where T : class, IEntity;
}

/// <summary>
/// Строит выражения поиска по строковым свойствам сущности
/// на основе путей, указанных в EntityView.SearchProperties.
/// </summary>
public class EntitySearchExpressionBuilder : IEntitySearchExpressionBuilder
{
    private static readonly MethodInfo ContainsMethod =
        typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })
        ?? throw new InvalidOperationException("Не удалось получить метод string.Contains(string).");

    private static readonly MethodInfo ToLowerMethod =
        typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)
        ?? throw new InvalidOperationException("Не удалось получить метод string.ToLower().");

    public Expression<Func<T, bool>> Build<T>(
        IReadOnlySet<string> searchProperties,
        string search)
    where T : class, IEntity
    {
        if (searchProperties.Count == 0)
            throw new ArgumentException("Список свойств поиска не может быть пустым.", nameof(searchProperties));

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? body = null;
        var normalizedSearch = search.ToLower();

        foreach (var path in searchProperties)
        {
            var propertyPredicate = BuildPropertyPredicate(parameter, path, normalizedSearch);
            // Условия по разным полям поиска объединяются через OR.
            body = body == null
                ? propertyPredicate
                : Expression.OrElse(body, propertyPredicate);
        }

        return Expression.Lambda<Func<T, bool>>(
            body ?? Expression.Constant(false),
            parameter);
    }

    /// <summary>
    /// Строит условие поиска для одного пути свойства.
    /// </summary>
    protected virtual Expression BuildPropertyPredicate(
        ParameterExpression parameter,
        string propertyPath,
        string search)
    {
        var pathAccess = PropertyExtractor.ExtractPropertyPathAccess(parameter, propertyPath);
        var accessExpression = pathAccess.AccessExpression;

        if (accessExpression.Type != typeof(string))
        {
            throw new InvalidOperationException(
                $"Свойство поиска \"{propertyPath}\" должно иметь тип string. Фактический тип: \"{accessExpression.Type.Name}\".");
        }

        // Сравнение без учёта регистра.
        var lowerAccess = Expression.Call(accessExpression, ToLowerMethod);
        var searchExpression = Expression.Constant(search);
        var containsExpression = Expression.Call(lowerAccess, ContainsMethod, searchExpression);

        // Для вложенных путей проверки на null должны идти перед Contains,
        // чтобы доступ к связанным объектам оставался безопасным.
        var predicate = pathAccess.NullChecks
            .Aggregate((Expression?)null, (acc, nullCheck)
                => acc == null ? nullCheck : Expression.AndAlso(acc, nullCheck));

        return predicate == null
            ? containsExpression
            : Expression.AndAlso(predicate, containsExpression);
    }


}
