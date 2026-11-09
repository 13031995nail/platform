namespace Tmobis.Business.Core.Models.Users;

using System;
using Tmobis.Core.Models;
using Tmobis.Core.Entities;
using Tmobis.Business.Core.Models.Towns;

public interface IUser
: IRegisteredEntity, IActualEntity, IWTown
{
    string Login { get; set; }

    string Name { get; set; }

    string? Email { get; set; }

    DateTime? PwdEditTime { get; set; }

    // [StrLen(50)]
    string? Pwd1 { get; set; }

    // [StrLen(50)]
    string? Pwd2 { get; set; }

    bool? IsComplexPwd { get; set; }

    string? UserId { get; set; }

    string? UserPwd { get; set; }

    public class Views
    {
        public static readonly EntityView UserE
            = new(
                nameof(UserE),
                [nameof(Town)],
                searchProperties: [
                    nameof(Name),
                    nameof(Login),
                    nameof(Email)
                ]);
    }
}