namespace Tmobis.Frameworks.Flexberry.Web.OData.Modules;
using NewPlatform.Flexberry.ORM.ODataService;
using Tmobis.Core.Modules;

public interface IODataModule : IModule
{
  /// <summary>
  /// Регистрация доп. методов и настроек OData.
  /// Например, ODataFunctions.
  /// </summary>
  /// <param name="manageToken">Токен настроек OData.</param>
  /// <param name="services">Сервис локатор.</param>
  public void ConfigureOData(
    ManagementToken manageToken,
    IServiceProvider services);
}
