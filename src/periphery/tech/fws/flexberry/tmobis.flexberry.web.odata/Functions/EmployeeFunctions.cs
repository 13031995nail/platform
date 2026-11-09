namespace Tmobis.Frameworks.Flexberry.Web.OData.Functions;
using System;
using System.Collections.Generic;
using Function = NewPlatform.Flexberry.ORM.ODataService.Functions.Function;
using Tmobis.Business.Core.Providers.Employees;
using Tmobis.Business.Core.Models.Employees;
using NewPlatform.Flexberry.ORM.ODataService.Functions;
using Tmobis.Core.Entities;

/// <summary>
/// ODataFunctions.
/// </summary>
public class EmployeeFunctions(
  IEmployeeProvider employeeProvider,
  IEntityMaterializer entityMaterializer)
: IEmployeeFunctions
{
  private readonly IEmployeeProvider employeeProvider = employeeProvider;
  private readonly IEntityMaterializer entityMaterializer = entityMaterializer;

  public virtual Type EmployeeType
  => entityMaterializer.ResolveStoredEntityType<IEmployee>();

  /// <summary>
  /// Получить OData функции.
  /// </summary>
  /// <returns>OData функции.</returns>
  public virtual List<Function> GetODataFunctions()
  {
    return
    [
        new ("GetEmployeeById", GetEmployeeById, EmployeeType,
            new Dictionary<string, Type>
            {
                { "id", typeof(Guid) },
            }),
      ];
  }

  /// <summary>
  /// Поиск Employee по его идентификатору.
  /// </summary>
  /// <param name="queryParameters">Параметры OData.</param>
  /// <param name="parameters">Парметры запроса.</param>
  public virtual IEnumerable<IEmployee> GetEmployeeById(
      QueryParameters queryParameters,
      IDictionary<string, object> parameters)
  {
    if (parameters == null)
    {
      return [];
    }

    var task = employeeProvider.GetById((Guid)parameters["id"]);
    task.Wait();
    var employee = task.Result;
    return employee != null
      ? [employee]
      : [];
  }
}
