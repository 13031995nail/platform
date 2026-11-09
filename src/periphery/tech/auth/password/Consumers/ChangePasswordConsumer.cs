namespace Tmobis.Auth.Password.Consumers;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Models;
using Providers;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Business.Core.Models.UsersPassword;
using Tmobis.Business.Core.Providers.Employees;
using Tmobis.Business.Core.Providers.Users;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Passwords;
using Tmobis.Core.Repositories;
using Verifiers;

public class ChangePasswordConsumer (
    IEmployeeProvider employeeProvider,
    IUserProvider userProvider,
    IEnumerable<IUserVerifier> verifiers,
    IRepository repository,
    IUnitOfWork unitOfWork,
    IUserPasswordProvider userPasswordProvider,
    ILogger<SecurityChangePasswordRequest> logger)
  : IMessageConsumer<SecurityChangePasswordRequest, SecurityChangePasswordResponse>
{
    private readonly IUserVerifier passwordHashVerifier = verifiers.OfType<PasswordHashUserVerifier>().First();
    public async Task<SecurityChangePasswordResponse> ConsumeWithResult(SecurityChangePasswordRequest message)
    {
        try
        {
            if (message == null)
            {
                return new SecurityChangePasswordResponse(userPasswordProvider.ErrorChangeIsComplexPassword);
            }

            if (string.IsNullOrWhiteSpace(message.NewPwd)
                || string.IsNullOrWhiteSpace(message.OldPwd)
                || message.EmployeeId.Equals(Guid.Empty))
            {
                return new SecurityChangePasswordResponse(userPasswordProvider.ErrorChangePasswordNullValues);
            }

            var employee = await employeeProvider.GetById(message.EmployeeId);
            if (employee == null || employee.User == null)
            {
                return new SecurityChangePasswordResponse(userPasswordProvider.ErrorChangeIsComplexPassword);
            }

            var user = await userProvider.GetByEmployee(employee);
            if (user == null)
            {
                return new SecurityChangePasswordResponse(userPasswordProvider.ErrorChangeIsComplexPassword);
            }

            if (!await passwordHashVerifier.Verify(user, message.OldPwd))
            {
                return new SecurityChangePasswordResponse(userPasswordProvider.ErrorChangePasswordInvalidPassword);
            }

            var pwds = repository.Query<IUserPassword>()
                .Where(x => x.UserId == user.Id).ToList();

            if (userPasswordProvider.IsEqualsToOldPwds(message.NewPwd, pwds))
            {
                return new SecurityChangePasswordResponse(userPasswordProvider.ErrorChangeOldPassword);
            }

            userPasswordProvider.UpdateUserPassword(user, message.NewPwd);
            await unitOfWork.Commit();
            return new SecurityChangePasswordResponse();
        }
        catch(Exception ex)
        {
            logger.LogError(ex, @"Внутренняя ошибка обработки");
            return new SecurityChangePasswordResponse(userPasswordProvider.ErrorChangeIsComplexPassword);
        }
    }
}
