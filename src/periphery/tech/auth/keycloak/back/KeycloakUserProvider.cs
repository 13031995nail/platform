namespace Tmobis.Auth.Keycloak;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Options;
using Tmobis.Business.Core.Models.Employees;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Business.Core.Providers.Employees;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Business.Core.Providers.Towns;
using Tmobis.Business.Core.Providers.Users;
using Tmobis.Core.Repositories;

/// <inheritdoc/>
public class KeycloakUserProvider(
    IRepository repository,
    IUserProvider userProvider,
    IEmployeeProvider employeeProvider,
    ITownProvider townProvider,
    IRoleProvider roleProvider,
    IOptions<KeycloakAuthOptions> options,
    ILogger<KeycloakUserProvider> logger)
: IKeycloakUserProvider
{
    private readonly KeycloakAuthOptions options = options.Value;

    /// <inheritdoc/>
    public async Task<IUser> GetOrCreateUser(
        Dictionary<string, string> claims, List<string> roles)
    {
        var (login, email, displayName, townId) = GetUserDataFromClaims(claims);
        var user = await GetOrCreateUserInternal(login, townId, email, displayName);
        await SyncRoles(user, roles);
        await CreateEmployeeIfNotExists(user);

        logger.LogInformation("Пользователь готов: {Login} (ID: {UserId})", login, user.Id);
        return user;
    }

    private UserData GetUserDataFromClaims(
        Dictionary<string, string> claims)
    {
        var login = claims.GetValueOrDefault(options.UsernameClaimType)
            ?? claims.GetValueOrDefault("email") 
            ?? claims.GetValueOrDefault("sub") 
            ?? throw new InvalidOperationException("У пользователя не определен логин");
    
        var email = claims.GetValueOrDefault("email");
        var displayName = claims.GetValueOrDefault("name")
            ?? claims.GetValueOrDefault("preferred_username");
        var townIdStr = claims.GetValueOrDefault("townId")
            ?? throw new InvalidOperationException("У пользователя не определен филиал");
        var townId = Guid.Parse(townIdStr);
    
        return new UserData(login, email, displayName, townId);
    }

    private async Task<IUser> GetOrCreateUserInternal(
        string login, Guid townId,
        string? email, string? displayName)
    {
        logger.LogInformation("Попытка получить или создать пользователя: {Login}", login);

        var user = await userProvider.GetUserByLogin(login);
        if (user != null)
            return UpdateUserIfChanged(user, email, displayName);

        if (options.EnableUserSync)
            return await CreateUser(login, townId, email, displayName);
        
        logger.LogWarning("Синхронизация пользователей отключена. Пользователь {Login} не найден.", login);
        throw new InvalidOperationException($"Пользователь {login} не найден в системе");
    }

    private async Task CreateEmployeeIfNotExists(IUser user)
    {
        var existingEmployee = await employeeProvider.GetByUser(user);
        if (existingEmployee == null)
        {
            logger.LogInformation("Создание Employee для пользователя: {Login}", user.Login);
            CreateEmployee(user);
        }
    }

    private async Task<IUser> CreateUser(
        string login,
        Guid townId,
        string? email = null,
        string? displayName = null)
    {
        logger.LogInformation("Создание нового пользователя: {Login}", login);
        
        var town = await townProvider.GetTownById(townId)
            ?? throw new InvalidOperationException("Не удалось определить филиал нового пользователя");

        var user = repository.MakeEntity<IUser>();
        user.Login = login;
        user.Town = town;
        user.Actuality = true;
        user.Email = email;
        user.Name = displayName ?? "Неизвестно";
        repository.Create(user);

        logger.LogInformation("Пользователь создан: {Login} (ID: {UserId})", login, user.Id);
        return user;
    }

    private IUser UpdateUserIfChanged(IUser user, string? email, string? displayName)
    {
        var needsUpdate = false;

        if (!string.IsNullOrEmpty(email))
        {
            var currentEmail = user.Email;
            if (string.IsNullOrEmpty(currentEmail) || currentEmail != email)
            {
                user.Email = email;
                needsUpdate = true;
            }
        }

        if (!string.IsNullOrEmpty(displayName))
        {
            var currentName = user.Name;
            if (string.IsNullOrEmpty(currentName) || currentName != displayName)
            {
                user.Name = displayName;
                needsUpdate = true;
            }
        }

        if (needsUpdate)
        {
            logger.LogInformation("Обновлены данные пользователя: {Login}", user.Login);
        }
        
        return user;
    }

    private async Task SyncRoles(IUser user, List<string> roles)
    {
        if (roles.Count == 0)
            return;
        
        var currentRoles = await roleProvider.GetRoleNamesByUser(user);
        var currentRolesSet = new HashSet<string>(currentRoles);

        foreach (var role in roles)
        {
            if (currentRolesSet.Contains(role)) continue;
            logger.LogInformation("Добавление роли {Role} пользователю {Login}", role, user.Login);
            // TODO: Нет смысла в EF, там EmptyRoleProvider.
            await roleProvider.GrantRole(user, role);
        }

        // TODO: Можно добавить логику удаления ролей, если они не существуют в Keycloak
        // TODO: Но лучше оставить управление ролями в системе, чтобы не потерять ручные изменения
    }

    private void CreateEmployee(IUser user)
    {
        var employee = repository.MakeEntity<IEmployee>();
        employee.User = user;
        employee.Actuality = true;
        employee.LastName = user.Name;
        employee.Town = user.Town;
        repository.Create(employee);

        logger.LogInformation("Создан Employee для пользователя: {Login}", user.Login);
    }
}

internal record UserData(
    string Login,
    string? Email,
    string? DisplayName,
    Guid TownId)
{
    public void Deconstruct(
        out string login,
        out string? email,
        out string? displayName,
        out Guid townId)
    {
        login = Login;
        email = Email;
        displayName = DisplayName;
        townId = TownId;
    }
}