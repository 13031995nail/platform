namespace Tmobis.Auth.Password.Providers;

using System.Collections.Generic;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Business.Core.Models.UsersPassword;

public interface IUserPasswordProvider
{
    string ErrorChangePasswordNullValues { get; }
    string ErrorChangePasswordInvalidPassword { get; }
    string ErrorChangeIsComplexPassword { get; }
    string ErrorChangeOldPassword { get; }
    string ErrorChangeUserCantChangePassword { get; }

    /// <summary>
    /// Обновить пользователя и пароль пользователя в БД
    /// </summary>
    /// <param name="user">Пользовтель.</param>
    /// <param name="newPwd">Новый пароль пользователя.</param>
    /// <param name="pwds">Предыдущие пароли пользователя.</param>
    /// <returns>Объект пользователя.</returns>
    public void UpdateUserPassword(IUser user, string? newPwd = null, List<IUserPassword>? pwds = null);

    /// <summary>
    /// Проверка, был ли такой пароль уже ранее использован
    /// </summary>
    /// <param name="newPwd">Новый пароль пользователя.</param>
    /// <param name="pwds">Предыдущие пароли пользователя.</param>
    public bool IsEqualsToOldPwds(string newPwd, List<IUserPassword> pwds);
}
