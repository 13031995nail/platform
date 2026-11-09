namespace Tmobis.Frameworks.Flexberry.Web.OData.Access;

using ICSSoft.STORMNET.FunctionalLanguage;

/// <summary>
/// Ограничение чтения сущности для OData-запроса.
/// </summary>
public sealed record ODataReadRestriction(
    Function? LimitFunction,
    IReadOnlyCollection<string> RequiredProperties)
{
    public const string PrimaryKeyProperty = "STORMMainObjectKey";

    public static ODataReadRestriction ForProperty(
        Function? limitFunction,
        string propertyPath)
        => new(
            limitFunction,
            limitFunction == null || propertyPath == PrimaryKeyProperty
                ? []
                : [propertyPath]);
}

/// <summary>
/// Поставщик доменного ограничения чтения для OData.
/// </summary>
public interface IODataReadRestrictionProvider
{
    bool Supports(Type entityType);

    Task<ODataReadRestriction> GetRestriction(Type entityType);
}
