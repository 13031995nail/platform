namespace Tmobis.Auth.Password.Consumers;

using System;
using Core.Repositories;
using Microsoft.Extensions.Logging;
using Models;
using Providers;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Business.Core.Models.UsersPassword;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Business.Core.Providers.Users;
using Tmobis.Core.Medium.Consumers;

public class SetIsComplexPasswordConsumer (
    IUnitOfWork unitOfWork,
    IUserProvider userProvider,
    ICurrentRoleProvider currentRoleProvider,
    ICurrentUserProvider currentUserProvider,
    IUserPasswordProvider userPasswordProvider,
    ILogger<SetIsComplexPasswordRequest> logger)
: IMessageConsumer<SetIsComplexPasswordRequest, SetIsComplexPasswordResponse>
{
    public async Task<SetIsComplexPasswordResponse> ConsumeWithResult(SetIsComplexPasswordRequest message)
    {
        try
        {
            if (message == null
                || string.IsNullOrWhiteSpace(message.Pwd)
                || message.UserId.Equals(Guid.Empty))
            {
                return new SetIsComplexPasswordResponse(userPasswordProvider.ErrorChangeIsComplexPassword);
            }

            var user = await userProvider.GetUserById(message.UserId);
            var currentUser = await currentUserProvider.GetCurrentUser();
            var currentUserRoles = await currentRoleProvider.GetCurrentRoleNames();
            var currentUserAdmin = currentUserRoles.Contains("admin")
                || currentUserRoles.Contains("Администратор");

            if (user == null)
            {
                return new SetIsComplexPasswordResponse(userPasswordProvider.ErrorChangeIsComplexPassword);
            }

            if (currentUser.Id != message.UserId && !currentUserAdmin)
            {
                return new SetIsComplexPasswordResponse(userPasswordProvider.ErrorChangeUserCantChangePassword);
            }

            userPasswordProvider.UpdateUserPassword(user, message.Pwd);
            await unitOfWork.Commit();
            return new SetIsComplexPasswordResponse();
        }
        catch(Exception ex)
        {
            logger.LogError(ex, @"Внутренняя ошибка обработки");
            return new SetIsComplexPasswordResponse(userPasswordProvider.ErrorChangeIsComplexPassword);
        }
    }
}
