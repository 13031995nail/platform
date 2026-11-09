namespace Tmobis.Frameworks.Flexberry.Web.OData.Functions;
using System;
using System.Collections.Generic;
using Function = NewPlatform.Flexberry.ORM.ODataService.Functions.Function;
using Tmobis.Business.Core.Models.Employees;
using NewPlatform.Flexberry.ORM.ODataService.Functions;

/// <summary>
/// ODataFunctions.
/// </summary>
public interface IEmployeeFunctions
{
  public Type EmployeeType { get; }

  /// <summary>
  /// Получить OData функции.
  /// </summary>
  /// <returns>OData функции.</returns>
  public List<Function> GetODataFunctions();

  /// <summary>
  /// Поиск Employee по его идентификатору.
  /// </summary>
  /// <param name="queryParameters">Параметры OData.</param>
  /// <param name="parameters">Парметры запроса.</param>
  public IEnumerable<IEmployee> GetEmployeeById(
      QueryParameters queryParameters,
      IDictionary<string, object> parameters);
}
