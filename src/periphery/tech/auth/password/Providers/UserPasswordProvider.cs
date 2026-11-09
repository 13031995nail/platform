namespace Tmobis.Auth.Password.Providers;

using Tmobis.Business.Core.Models.Users;
using Tmobis.Business.Core.Models.UsersPassword;
using Tmobis.Business.Core.Providers.Users;
using Tmobis.Core.Passwords;
using Tmobis.Core.Repositories;

public class UserPasswordProvider(
    IRepository repository,
    IPasswordHasher passwordHasher
) : IUserPasswordProvider
{
    public string ErrorChangePasswordNullValues => "Значения паролей не могут быть пустыми. Попробуйте снова.";
    public string ErrorChangePasswordInvalidPassword => "Неверный предыдущий пароль. Попробуйте снова.";
    public string ErrorChangeIsComplexPassword => "Произошла ошибка при сохранении. Попробуйте снова.";
    public string ErrorChangeOldPassword => "Пароль повторяется с одним из пяти предыдущих, придумайте новый пароль.";
    public string ErrorChangeUserCantChangePassword => "Невозможно изменить пароль данному пользователю.";

    public void UpdateUserPassword(IUser user, string? newPwd = null, List<IUserPassword>? pwds = null)
    {
        if (newPwd != null)
        {
            var hashedPwd = passwordHasher.Hash(newPwd);
            user.UserPwd = hashedPwd;
            user.PwdEditTime = DateTime.Now;

            var userPassword = repository.MakeEntity<IUserPassword>();
            var isNewPassword = true;

            if (pwds?.Count == 5)
            {
                userPassword = pwds.OrderByDescending(x => x.CreateTime).First();
                isNewPassword = false;
            }

            userPassword.CreateTime = DateTime.Now;
            userPassword.Creator = user.Id.ToString();
            userPassword.PwdHash = hashedPwd;
            userPassword.UserId = user.Id;

            if (isNewPassword)
            {
                repository.Create(userPassword);
            }
            else
            {
                repository.Update(userPassword);
            }
        }

        user.IsComplexPwd = true;
        repository.Update(user);
    }

    public bool IsEqualsToOldPwds(string newPwd, List<IUserPassword> pwds)
    => pwds.Any(
        x => passwordHasher.VerifyPassword(
            x.PwdHash, newPwd));
}
