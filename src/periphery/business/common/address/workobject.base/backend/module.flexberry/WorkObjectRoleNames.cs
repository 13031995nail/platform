namespace Tmobis.Business.Common.Address;

using Tmobis.Business.Core.Providers.Roles;

/// <summary>
/// Наименования ролей, определяющих доступ к объектам работы.
/// </summary>
public static class WorkObjectRoleNames
{
    public const string Consumer = "Потребитель";
    public const string Tso = "ТСО";
    public const string Administration = BusinessRoleNames.Administration;
    public const string Admin = BusinessRoleNames.Admin;
    public const string Administrator = BusinessRoleNames.Administrator;
}
