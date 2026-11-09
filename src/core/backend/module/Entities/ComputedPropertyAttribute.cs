namespace Tmobis.Core.Entities;

/// <summary>
/// Атрибут для вычислимых свойств в интерфейс-сущностях.
/// Содержит имя метода, который нужно вызывать в геттере свойства.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ComputedPropertyAttribute : Attribute
{
    /// <summary>
    /// Полное имя метода (с указанием класса и неймспейса),
    /// который должен вызываться в геттере вычислимого свойства.
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// Создание нового экземпляра атрибута с указанным именем метода.
    /// </summary>
    /// <param name="methodName">Имя метода для вызова в геттере вычислимого свойства.</param>
    public ComputedPropertyAttribute(string methodName)
    {
        MethodName = methodName;
    }
}