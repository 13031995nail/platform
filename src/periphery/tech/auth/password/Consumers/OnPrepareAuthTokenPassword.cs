namespace Tmobis.Auth.Password.Consumers;

using Tmobis.Auth;
using Tmobis.Core.Medium.Consumers;

public class OnPrepareAuthTokenPassword
: IMessageConsumer<PrepareAuthToken, PrepareAuthTokenResult>
{
    public async Task<PrepareAuthTokenResult>
        ConsumeWithResult(PrepareAuthToken message)
    {
        var user = message.User;
        return new(
            [],
            new Dictionary<string, string>
            {
                ["is_old_pwd"] = (user.PwdEditTime != null
                    && DateTime.Now - user.PwdEditTime
                    > new TimeSpan(180, 0, 0, 0)
                ).ToString(),
                ["is_complex_pwd"] = (user.IsComplexPwd ?? false).ToString()
            }
        )
        { Continue = true };
    }
}
