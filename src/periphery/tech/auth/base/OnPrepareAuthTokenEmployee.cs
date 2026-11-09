namespace Tmobis.Auth;

using System.Security.Claims;
using Tmobis.Business.Core.Providers.Employees;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Types;

public class OnPrepareAuthTokenEmployee(
    IEmployeeProvider employeeProvider)
: IMessageConsumer<PrepareAuthToken, PrepareAuthTokenResult>
{
    public async Task<PrepareAuthTokenResult>
        ConsumeWithResult(PrepareAuthToken message)
    {
        var employee = await employeeProvider.GetByUser(message.User)
            ?? throw new InvalidOperationException();
        return new(
    [new(ClaimTypes.NameIdentifier, employee.Id.ToString())],
    new Dictionary<string, string> { ["employee"] = employee.Id.ToStringTrim() }
        )
        { Continue = true };
    }
}
