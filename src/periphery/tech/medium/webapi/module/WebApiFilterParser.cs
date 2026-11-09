namespace Tmobis.Medium.WebApi;

using Core.Entities.Filters;
using Core.Converters;

public class WebApiFilterParser(
    IPropertyNameConverter propertyNameConverter)
: IFilterParser
{
    public virtual EntityQueryFilter Parse(string[] filter)
    {
        return new EntityQueryFilter(filter.Select(ParseOperation).ToList());
    }

    protected virtual EntityQueryFilterOperation ParseOperation(string rawFilter)
    {
        if (string.IsNullOrWhiteSpace(rawFilter))
            throw new ArgumentException("Фильтр не может быть пустым.");

        var firstSeparatorIndex = rawFilter.IndexOf(';');
        if (firstSeparatorIndex < 0)
            throw new ArgumentException($"Фильтр \"{rawFilter}\" некорректен: отсутствует первый разделитель ';'.");

        var secondSeparatorIndex = rawFilter.IndexOf(';', firstSeparatorIndex + 1);
        if (secondSeparatorIndex < 0)
            throw new ArgumentException($"Фильтр \"{rawFilter}\" некорректен: отсутствует второй разделитель ';'.");

        var propertyName = rawFilter[..firstSeparatorIndex].Trim();
        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentException($"Фильтр \"{rawFilter}\" некорректен: имя свойства не может быть пустым.");

        var operationRaw = rawFilter[(firstSeparatorIndex + 1)..secondSeparatorIndex].Trim();
        if (string.IsNullOrWhiteSpace(operationRaw))
            throw new ArgumentException($"Фильтр \"{rawFilter}\" некорректен: оператор не может быть пустым.");

        var value = rawFilter[(secondSeparatorIndex + 1)..].Trim();
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"Фильтр \"{rawFilter}\" некорректен: значение не может быть пустым.");

        return new EntityQueryFilterOperation
        {
            PropertyName = propertyNameConverter.GetBackendPropertyName(propertyName),
            Operator = ParseOperator(operationRaw, propertyName),
            Value = value
        };
    }

    protected virtual FilterOperation ParseOperator(string operation, string propertyName)
    {
        return operation.ToLowerInvariant() switch
        {
            "eq" => FilterOperation.Equal,
            "neq" => FilterOperation.NotEqual,
            "contains" => FilterOperation.Contains,
            "gte" => FilterOperation.GreaterThanOrEqual,
            "lte" => FilterOperation.LesserThanOrEqual,
            "gt" => FilterOperation.GreaterThan,
            "lt" => FilterOperation.LesserThan,
            _ => throw new ArgumentException(
                $"Оператор фильтра \"{operation}\" для свойства \"{propertyName}\" не поддерживается.")
        };
    }
}
