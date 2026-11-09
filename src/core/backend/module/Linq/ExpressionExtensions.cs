namespace Tmobis.Core.Linq;

using System.Linq.Expressions;

/// <summary>
/// Содержит вспомогательные методы для работы с выражениями условий.
/// </summary>
public static class ExpressionExtensions
{
    extension<T>(Expression<Func<T, bool>> left)
    {
        /// <summary>
        /// Объединяет текущее и переданное условие через логическое И.
        /// </summary>
        public Expression<Func<T, bool>> AndAlso(Expression<Func<T, bool>> right)
            => Combine(left, right, Expression.AndAlso);

        /// <summary>
        /// Объединяет текущее и переданное условие через логическое ИЛИ.
        /// </summary>
        public Expression<Func<T, bool>> OrElse(Expression<Func<T, bool>> right)
            => Combine(left, right, Expression.OrElse);
    }

    /// <summary>
    /// Объединяет два выражения без Expression.Invoke, чтобы итоговое выражение
    /// оставалось доступным для разбора поставщиками LINQ-запросов.
    /// Правое выражение предварительно перепривязывается к параметру левого выражения.
    /// </summary>
    private static Expression<Func<T, bool>> Combine<T>(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right,
        Func<Expression, Expression, BinaryExpression> merge)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        ArgumentNullException.ThrowIfNull(merge);

        var parameter = left.Parameters[0];
        var rightBody = new ReplaceExpressionVisitor(right.Parameters[0], parameter).Visit(right.Body)
                        ?? throw new InvalidOperationException("Не удалось объединить выражения.");

        var body = merge(left.Body, rightBody);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private sealed class ReplaceExpressionVisitor(Expression source, Expression target)
        : ExpressionVisitor
    {
        public override Expression? Visit(Expression? node)
        {
            return node == source ? target : base.Visit(node);
        }
    }
}
